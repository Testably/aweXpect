using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     A field or property of a type, resolved either from the <see cref="TypeMetadataRegistry" /> or by reflection.
/// </summary>
internal readonly struct EquivalencyMember(
	string name,
	Type declaredType,
	Func<object, object?> getValue,
	TypeMetadataRegistry.ValueComparer? valueComparer = null)
{
	public string Name { get; } = name;

	/// <remarks>
	///     A <see cref="Nullable{T}" /> is unwrapped, because the member holds a value of the underlying type, which is
	///     also the runtime type a collection item of that type has.
	/// </remarks>
	public Type DeclaredType { get; } = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
	public Func<object, object?> GetValue { get; } = getValue;

	/// <summary>
	///     Compares the member on two objects of the registered type without boxing, when it has a primitive or enum
	///     type.
	/// </summary>
	public TypeMetadataRegistry.ValueComparer? ValueComparer { get; } = valueComparer;
}

/// <summary>
///     Resolves the members that participate in an equivalency comparison.
/// </summary>
/// <remarks>
///     A registered type is served from the <see cref="TypeMetadataRegistry" />, so that publishing with trimming or
///     Native AOT enabled does not remove its members. Only public members are registered, so a comparison that asks
///     for non-public members reflects over the whole type: mixing the two sources would let a registered public
///     member stand next to the non-public member that hides it, which reflection alone never does.
/// </remarks>
internal static class EquivalencyMembers
{
	/// <remarks>
	///     The members of a type are read for every compared object, so the members of a type that is not registered
	///     and their accessors are created once per type and visibility.
	/// </remarks>
	private static readonly ConcurrentDictionary<(Type, IncludeMembers), EquivalencyMember[]> ReflectedFields = new();

	private static readonly ConcurrentDictionary<(Type, IncludeMembers), EquivalencyMember[]> ReflectedProperties =
		new();

	private static readonly ConcurrentDictionary<(Type, string, IncludeMembers), Func<object, object?>?>
		ReflectedFieldAccessors = new();

	private static readonly ConcurrentDictionary<(Type, string, IncludeMembers), Func<object, object?>?>
		ReflectedPropertyAccessors = new();

	public static EquivalencyMember[] GetFields(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.OrderedMembers.Fields;
		}

		return ReflectedFields.GetOrAdd((type, includeMembers), static key => key.Item1.GetFields(key.Item2)
			.Select(field => new EquivalencyMember(field.Name, field.FieldType, Accessor(field)))
			.ToArray());
	}

	public static EquivalencyMember[] GetProperties(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.OrderedMembers.Properties;
		}

		return ReflectedProperties.GetOrAdd((type, includeMembers), static key => key.Item1.GetProperties(key.Item2)
			.Select(property => new EquivalencyMember(property.Name, DeclaredType(property), Accessor(property)))
			.ToArray());
	}

	/// <summary>
	///     Returns the accessor for the field <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindField(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Fields.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member)
				? member.GetValue
				: null;
		}

		return ReflectedFieldAccessors.GetOrAdd((type, name, includeMembers), static key
			=> key.Item1.FindField(key.Item2, key.Item3) is { } field ? Accessor(field) : null);
	}

	/// <summary>
	///     Returns the accessor for the property <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindProperty(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.Properties.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member)
				? member.GetValue
				: null;
		}

		return ReflectedPropertyAccessors.GetOrAdd((type, name, includeMembers), static key
			=> key.Item1.FindProperty(key.Item2, key.Item3) is { } property ? Accessor(property) : null);
	}

	/// <summary>
	///     Returns the accessor for the property that the <paramref name="type" /> implements explicitly for an
	///     interface property <paramref name="name" />, or <see langword="null" /> when it has none or, which
	///     <paramref name="isAmbiguous" /> then reports, more than one.
	/// </summary>
	/// <remarks>
	///     The interface makes an explicit implementation public whatever the visibility of its getter, so it is found
	///     for any visibility except <see cref="IncludeMembers.None" />.
	/// </remarks>
	public static Func<object, object?>? FindExplicitProperty(Type type, string name, IncludeMembers includeMembers,
		out bool isAmbiguous)
	{
		isAmbiguous = false;
		if (includeMembers == IncludeMembers.None)
		{
			return null;
		}

		string suffix = "." + name;
		List<Func<object, object?>> accessors = TryGetRegistered(type, includeMembers,
			out TypeMetadataRegistry.TypeMetadata? metadata)
			? metadata.ExplicitProperties.Values
				.Where(member => member.Name.EndsWith(suffix, StringComparison.Ordinal))
				.Select(member => member.GetValue)
				.ToList()
			: type.GetExplicitProperties()
				.Where(property => property.Name.EndsWith(suffix, StringComparison.Ordinal))
				.Select(Accessor)
				.ToList();
		isAmbiguous = accessors.Count > 1;
		return accessors.Count == 1 ? accessors[0] : null;
	}

	/// <summary>
	///     Whether the public members of the <paramref name="type" /> are registered.
	/// </summary>
	public static bool IsRegistered(Type type)
		=> TryGetRegistered(type, IncludeMembers.Public, out _);

	/// <remarks>
	///     A type counts as registered only when it has a field or a property: an event-only registration says nothing
	///     about the members, so such a type is reflected over like an unregistered one.
	/// </remarks>
	private static bool TryGetRegistered(Type type, IncludeMembers includeMembers,
		[NotNullWhen(true)] out TypeMetadataRegistry.TypeMetadata? metadata)
	{
		if (includeMembers != IncludeMembers.Public)
		{
			metadata = null;
			return false;
		}

		return TypeMetadataRegistry.Instance.TryGet(type, out metadata) && metadata.OrderedMembers.HasMembers;
	}

	/// <remarks>
	///     Reflection declares a <see langword="ref" />-returning property with the by-ref type, while its value and a
	///     registration of it have the type it refers to.
	/// </remarks>
	private static Type DeclaredType(PropertyInfo property)
		=> property.PropertyType.IsByRef ? property.PropertyType.GetElementType()! : property.PropertyType;

	private static Func<object, object?> Accessor(FieldInfo field)
		=> subject => Read(static (field, subject) => field.GetValue(subject), field, subject);

	private static Func<object, object?> Accessor(PropertyInfo property)
		=> subject => Read(static (property, subject) => property.GetValue(subject), property, subject);

	/// <remarks>
	///     Reflection wraps an exception thrown by a getter, while a registered accessor lets it through. Unwrapping
	///     keeps the two paths indistinguishable to the caller.
	/// </remarks>
	private static object? Read<TMember>(Func<TMember, object, object?> read, TMember member, object subject)
	{
		try
		{
			return read(member, subject);
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}
}
