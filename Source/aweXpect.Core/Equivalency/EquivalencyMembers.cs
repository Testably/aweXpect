using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using aweXpect.Core;
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

	private static readonly ConcurrentDictionary<(Type, IncludeMembers), EquivalencyMember[]> ReflectedOwnFields =
		new();

	private static readonly ConcurrentDictionary<(Type, IncludeMembers), EquivalencyMember[]>
		ReflectedOwnProperties = new();

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
	///     Returns the fields that the collection <paramref name="type" /> declares itself, which are compared in
	///     addition to its items.
	/// </summary>
	/// <remarks>
	///     A field or property with a requested visibility is a member of the collection type itself, unless<br />
	///     - a type of the framework declares it, which is a type in the namespace <c>System</c> or <c>Microsoft</c>
	///     or in a namespace nested in one of them, where a property that overrides another one counts as declared by
	///     the type that declared it first,<br />
	///     - it is a property that implements a property of an interface of the framework, such as the <c>Count</c>
	///     of an <see cref="IReadOnlyCollection{T}" />, or<br />
	///     - the compiler generated the field or the type that declares the member, such as the captured state of an
	///     iterator.<br />
	///     These members describe the collection, which the comparison of the items covers.
	///     <c>TypeMetadataGenerator.OwnMembers</c> mirrors this rule for the registrations.
	///     <para />
	///     A type that is registered as a collection is served from the <see cref="TypeMetadataRegistry" />, which
	///     holds exactly these members. Without the reflection fallback, any other type has none, because a
	///     collection that only reaches the comparison through its runtime type, such as an iterator, cannot be
	///     registered.
	/// </remarks>
	public static EquivalencyMember[] GetOwnFields(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegisteredCollection(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.OrderedMembers.Fields;
		}

		return ReflectedOwnFields.GetOrAdd((type, includeMembers), static key
			=> ReflectionFallback.IsSupported
				? key.Item1.GetFields(key.Item2)
					.Where(IsOwnField)
					.Select(field => new EquivalencyMember(field.Name, field.FieldType, Accessor(field)))
					.ToArray()
				: []);
	}

	/// <summary>
	///     Returns the properties that the collection <paramref name="type" /> declares itself, which are compared in
	///     addition to its items.
	/// </summary>
	/// <inheritdoc cref="GetOwnFields" path="/remarks" />
	public static EquivalencyMember[] GetOwnProperties(Type type, IncludeMembers includeMembers)
	{
		if (TryGetRegisteredCollection(type, includeMembers, out TypeMetadataRegistry.TypeMetadata? metadata))
		{
			return metadata.OrderedMembers.Properties;
		}

		return ReflectedOwnProperties.GetOrAdd((type, includeMembers), static key
			=> ReflectionFallback.IsSupported ? ReflectOwnProperties(key.Item1, key.Item2) : []);
	}

	/// <summary>
	///     Returns the accessor for the member <paramref name="name" /> that is registered for the collection
	///     <paramref name="type" />, looked up by its own kind first, or <see langword="null" /> when the type is not
	///     registered as a collection or declares no such member itself.
	/// </summary>
	public static Func<object, object?>? FindOwnMember(Type type, string name, bool isField, IncludeMembers fields,
		IncludeMembers properties)
	{
		Func<object, object?>? field =
			TryGetRegisteredCollection(type, fields, out TypeMetadataRegistry.TypeMetadata? metadata) &&
			metadata.Fields.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? registeredField)
				? registeredField.GetValue
				: null;
		Func<object, object?>? property =
			TryGetRegisteredCollection(type, properties, out metadata) &&
			metadata.Properties.TryGetValue(name, out TypeMetadataRegistry.RegisteredMember? registeredProperty)
				? registeredProperty.GetValue
				: null;
		return isField ? field ?? property : property ?? field;
	}

	/// <summary>
	///     Whether the <paramref name="type" /> or one of its base types is no type of the framework, so that it can
	///     declare members of its own.
	/// </summary>
	/// <remarks>
	///     An array reports the namespace of its element type, but is declared by the framework.
	/// </remarks>
	public static bool CanDeclareOwnMembers(Type type)
	{
		if (type.IsArray)
		{
			return false;
		}

		for (Type? current = type; current is not null; current = current.BaseType)
		{
			if (!IsFramework(current))
			{
				return true;
			}
		}

		return false;
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
	///     about the members, so such a type is reflected over like an unregistered one. So is a type that is
	///     registered as a collection, because only the members it declares itself are registered for it.
	/// </remarks>
	private static bool TryGetRegistered(Type type, IncludeMembers includeMembers,
		[NotNullWhen(true)] out TypeMetadataRegistry.TypeMetadata? metadata)
	{
		if (includeMembers != IncludeMembers.Public)
		{
			metadata = null;
			return false;
		}

		return TypeMetadataRegistry.Instance.TryGet(type, out metadata) && metadata.OrderedMembers.HasMembers &&
		       !metadata.IsCollection;
	}

	private static bool TryGetRegisteredCollection(Type type, IncludeMembers includeMembers,
		[NotNullWhen(true)] out TypeMetadataRegistry.TypeMetadata? metadata)
	{
		if (includeMembers != IncludeMembers.Public)
		{
			metadata = null;
			return false;
		}

		return TypeMetadataRegistry.Instance.TryGet(type, out metadata) && metadata.IsCollection;
	}

	private static bool IsOwnField(FieldInfo field)
		=> IsOwn(field.DeclaringType!) && !field.IsDefined(typeof(CompilerGeneratedAttribute), false);

	private static bool IsOwnProperty(PropertyInfo property, HashSet<(Module, int)> frameworkImplementations)
	{
		MethodInfo declaration = property.GetGetMethod(true)!.GetBaseDefinition();
		return IsOwn(declaration.DeclaringType!) &&
		       !frameworkImplementations.Contains((declaration.Module, declaration.MetadataToken));
	}

	private static bool IsOwn(Type declaringType)
		=> !IsFramework(declaringType) && !declaringType.IsDefined(typeof(CompilerGeneratedAttribute), false);

	private static bool IsFramework(Type type)
		=> type.Namespace is { } typeNamespace &&
		   (IsOrIsNestedIn(typeNamespace, "System") || IsOrIsNestedIn(typeNamespace, "Microsoft"));

	private static bool IsOrIsNestedIn(string typeNamespace, string root)
		=> typeNamespace.StartsWith(root, StringComparison.Ordinal) &&
		   (typeNamespace.Length == root.Length || typeNamespace[root.Length] == '.');

	/// <remarks>
	///     The methods that implement a method of an interface of the framework are identified by the module and the
	///     metadata token of their first declaration, which is the same whatever type a method was reflected through,
	///     and whichever override of it the interface is mapped to.
	/// </remarks>
#if NET8_0_OR_GREATER
	[RequiresUnreferencedCode("Reads the interface implementations of the type, which the trimmer may remove.")]
#endif
	private static EquivalencyMember[] ReflectOwnProperties(Type type, IncludeMembers includeMembers)
	{
		HashSet<(Module, int)> frameworkImplementations = new(type.GetInterfaces()
			.Where(IsFramework)
			.SelectMany(interfaceType => type.GetInterfaceMap(interfaceType).TargetMethods)
			.Select(method => method.GetBaseDefinition())
			.Select(declaration => (declaration.Module, declaration.MetadataToken)));
		return type.GetProperties(includeMembers)
			.Where(property => IsOwnProperty(property, frameworkImplementations))
			.Select(property => new EquivalencyMember(property.Name, DeclaredType(property), Accessor(property)))
			.ToArray();
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
