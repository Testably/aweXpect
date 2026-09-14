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
internal readonly struct EquivalencyMember(string name, Type declaredType, Func<object, object?> getValue)
{
	public string Name { get; } = name;
	public Type DeclaredType { get; } = declaredType;
	public Func<object, object?> GetValue { get; } = getValue;
}

/// <summary>
///     Resolves the members that participate in an equivalency comparison.
/// </summary>
/// <remarks>
///     The public members of a registered type are served from the <see cref="TypeMetadataRegistry" />, so that
///     publishing with trimming or Native AOT enabled does not remove them. Non-public members are never registered,
///     so they are reflected over like the members of an unregistered type.
/// </remarks>
internal static class EquivalencyMembers
{
	public static IEnumerable<EquivalencyMember> GetFields(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return Registered(metadata.Fields, includeMembers)
				.Concat(ReflectFields(type, includeMembers & ~IncludeMembers.Public));
		}

		return ReflectFields(type, includeMembers);
	}

	public static IEnumerable<EquivalencyMember> GetProperties(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return Registered(metadata.Properties, includeMembers)
				.Concat(ReflectProperties(type, includeMembers & ~IncludeMembers.Public));
		}

		return ReflectProperties(type, includeMembers);
	}

	/// <summary>
	///     Returns the accessor for the field <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindField(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.Fields.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member))
		{
			return member.GetValue;
		}

		FieldInfo? field = type.GetFields(includeMembers).FirstOrDefault(x => x.Name == name);
		return field is null ? null : Accessor(field);
	}

	/// <summary>
	///     Returns the accessor for the property <paramref name="name" /> on the <paramref name="type" />, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public static Func<object, object?>? FindProperty(Type type, string name, IncludeMembers includeMembers)
	{
		if (TryGetRegistered(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.Properties.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? member))
		{
			return member.GetValue;
		}

		PropertyInfo? property = type.GetProperties(includeMembers).FirstOrDefault(x => x.Name == name);
		return property is null ? null : Accessor(property);
	}

	/// <remarks>
	///     A type counts as registered only when it has a field or a property: an event-only registration says nothing
	///     about the members, so such a type is reflected over like an unregistered one.
	/// </remarks>
	private static bool TryGetRegistered(Type type,
		[NotNullWhen(true)] out TypeMetadataRegistry.TypeMetadata? metadata)
		=> TypeMetadataRegistry.Instance.TryGet(type, out metadata) &&
		   !(metadata.Fields.IsEmpty && metadata.Properties.IsEmpty);

	private static IEnumerable<EquivalencyMember> Registered(
		ConcurrentDictionary<string, TypeMetadataRegistry.RegisteredMember> members,
		IncludeMembers includeMembers)
		=> includeMembers.HasFlag(IncludeMembers.Public)
			? members.Values
				.OrderBy(member => member.Order)
				.Select(member => new EquivalencyMember(member.Name, member.MemberType, member.GetValue))
			: Enumerable.Empty<EquivalencyMember>();

	private static IEnumerable<EquivalencyMember> ReflectFields(Type type, IncludeMembers includeMembers)
		=> type.GetFields(includeMembers)
			.Select(field => new EquivalencyMember(field.Name, field.FieldType, Accessor(field)));

	private static IEnumerable<EquivalencyMember> ReflectProperties(Type type, IncludeMembers includeMembers)
		=> type.GetProperties(includeMembers)
			.Select(property => new EquivalencyMember(property.Name, property.PropertyType, Accessor(property)));

	private static Func<object, object?> Accessor(FieldInfo field)
		=> subject => Read(() => field.GetValue(subject));

	private static Func<object, object?> Accessor(PropertyInfo property)
		=> subject => Read(() => property.GetValue(subject));

	/// <remarks>
	///     Reflection wraps an exception thrown by a getter, while a registered accessor lets it through. Unwrapping
	///     keeps the two paths indistinguishable to the caller.
	/// </remarks>
	private static object? Read(Func<object?> read)
	{
		try
		{
			return read();
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}
}
