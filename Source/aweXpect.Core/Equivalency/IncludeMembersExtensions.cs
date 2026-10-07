using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace aweXpect.Equivalency;

internal static class IncludeMembersExtensions
{
	/// <remarks>
	///     The members of a type are looked up once per member of the compared object, so both the readable members
	///     and the visibility-filtered result are kept per type.
	/// </remarks>
	private static readonly ConcurrentDictionary<(Type, BindingFlags), FieldInfo[]> AllFields = new();

	private static readonly ConcurrentDictionary<(Type, BindingFlags), PropertyInfo[]> AllProperties = new();
	private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ExplicitProperties = new();
	private static readonly ConcurrentDictionary<(Type, IncludeMembers), FieldInfo[]> Fields = new();
	private static readonly ConcurrentDictionary<(Type, IncludeMembers), PropertyInfo[]> Properties = new();

	public static BindingFlags GetBindingFlags(this IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.Public)
		{
			return BindingFlags.Public | BindingFlags.Instance;
		}

#pragma warning disable S3011 // https://rules.sonarsource.com/csharp/RSPEC-3011
		return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
#pragma warning restore S3011
	}

	public static IEnumerable<FieldInfo> GetFields(this Type type, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None)
		{
			return Enumerable.Empty<FieldInfo>();
		}

		return Fields.GetOrAdd((type, includeMembers), static key
			=> GetAllFields(key.Item1, key.Item2)
				.Where(field => Includes(key.Item2, field.IsPublic, field.IsAssembly || field.IsFamilyOrAssembly))
				.ToArray());
	}

	public static IEnumerable<PropertyInfo> GetProperties(this Type type, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None)
		{
			return Enumerable.Empty<PropertyInfo>();
		}

		return Properties.GetOrAdd((type, includeMembers), static key
			=> GetAllProperties(key.Item1, key.Item2)
				.Where(property =>
				{
					MethodInfo getter = property.GetGetMethod(true)!;
					return Includes(key.Item2, getter.IsPublic, getter.IsAssembly || getter.IsFamilyOrAssembly);
				})
				.ToArray());
	}

	/// <summary>
	///     Finds the field <paramref name="name" /> the way a lookup with the binding flags of
	///     <paramref name="includeMembers" /> does, without requiring the field itself to have a requested visibility.
	/// </summary>
	/// <remarks>
	///     A field that is more visible than requested is still found, but a protected or private one never is. It
	///     still hides a base field of the same name, like it does for the expected object.
	/// </remarks>
	public static FieldInfo? FindField(this Type type, string name, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None ||
		    GetAllFields(type, includeMembers).FirstOrDefault(field => field.Name == name) is not { } field)
		{
			return null;
		}

		return Includes(includeMembers | IncludeMembers.Public, field.IsPublic,
			field.IsAssembly || field.IsFamilyOrAssembly)
			? field
			: null;
	}

	/// <summary>
	///     Finds the property <paramref name="name" /> the way a lookup with the binding flags of
	///     <paramref name="includeMembers" /> does, without requiring the property itself to have a requested
	///     visibility.
	/// </summary>
	/// <remarks>
	///     A public request only sees a property that can be read publicly, because a registration cannot call a
	///     non-public getter and the two paths have to agree. A property that is more visible than requested is still
	///     found, but one with a protected or private getter never is. It still hides a base property of the same
	///     name, like it does for the expected object.
	/// </remarks>
	public static PropertyInfo? FindProperty(this Type type, string name, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None ||
		    GetAllProperties(type, includeMembers).FirstOrDefault(property => property.Name == name) is not
			    { } property)
		{
			return null;
		}

		MethodInfo getter = property.GetGetMethod(true)!;
		return Includes(includeMembers | IncludeMembers.Public, getter.IsPublic,
			getter.IsAssembly || getter.IsFamilyOrAssembly)
			? property
			: null;
	}

	/// <summary>
	///     Returns the readable properties that the <paramref name="type" /> implements explicitly for an interface.
	/// </summary>
	/// <remarks>
	///     The compiler names an explicit implementation after its interface, such as <c>Namespace.IHasValue.Value</c>,
	///     and makes its getter private, so a lookup by the short name never finds it. Reflection does not return the
	///     private members of a base type either, so the hierarchy is walked, and a re-implementation on a derived type
	///     hides the one on its base.
	/// </remarks>
	public static PropertyInfo[] GetExplicitProperties(this Type type)
		=> ExplicitProperties.GetOrAdd(type, static key =>
		{
			if (!ReflectionFallback.IsSupported)
			{
				throw Tracing.WriteException(ReflectionFallback.NotSupported(key, "properties"));
			}

			Dictionary<string, PropertyInfo> byName = new(StringComparer.Ordinal);
			for (Type? current = key; current is not null && current != typeof(object); current = current.BaseType)
			{
#pragma warning disable S3011 // https://rules.sonarsource.com/csharp/RSPEC-3011
				foreach (PropertyInfo property in current.GetProperties(
					         BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
#pragma warning restore S3011
				{
					if (property.Name.Contains('.', StringComparison.Ordinal) && property.GetGetMethod(true) is { IsPrivate: true, } &&
					    property.GetIndexParameters().Length == 0 && !IsByRefLike(property.PropertyType) &&
					    !byName.ContainsKey(property.Name))
					{
						byName.Add(property.Name, property);
					}
				}
			}

			return byName.Values.ToArray();
		});

	private static FieldInfo[] GetAllFields(Type type, IncludeMembers includeMembers)
		=> AllFields.GetOrAdd((type, includeMembers.GetBindingFlags()), static key
			=> ReflectionFallback.IsSupported
				? MostDerived(key.Item1.GetFields(key.Item2))
				: throw Tracing.WriteException(ReflectionFallback.NotSupported(key.Item1, "fields")));

	/// <remarks>
	///     An indexer is a property whose getter takes arguments, so its value cannot be read for the comparison.<br />
	///     Readability is checked after the most derived declaration is taken, so that a declaration without a getter
	///     hides a base property of the same name just like one with a non-public getter does. A property of a
	///     by-ref-like type, such as a span, cannot be read either, because its value cannot be boxed.
	/// </remarks>
	private static PropertyInfo[] GetAllProperties(Type type, IncludeMembers includeMembers)
		=> AllProperties.GetOrAdd((type, includeMembers.GetBindingFlags()), static key
			=> ReflectionFallback.IsSupported
				? MostDerived(key.Item1.GetProperties(key.Item2)
						.Where(property => property.GetIndexParameters().Length == 0))
					.Select(WithInheritedGetter)
					.Where(property => property.CanRead && !IsByRefLike(property.PropertyType))
					.ToArray()
				: throw Tracing.WriteException(ReflectionFallback.NotSupported(key.Item1, "properties")));

	/// <remarks>
	///     netstandard2.0 has no <c>Type.IsByRefLike</c>, but is served to runtimes that have by-ref-like types, which
	///     the compiler marks with the <c>IsByRefLikeAttribute</c>.
	/// </remarks>
	private static bool IsByRefLike(Type type)
#if NET8_0_OR_GREATER
		=> type.IsByRefLike;
#else
		=> type.IsValueType && type.GetCustomAttributesData().Any(attribute
			=> attribute.AttributeType.FullName == "System.Runtime.CompilerServices.IsByRefLikeAttribute");
#endif

	/// <remarks>
	///     An override that declares only a setter still inherits the getter, but reflection returns the override
	///     without it, so the property is read through the declaration whose setter it overrides.
	/// </remarks>
#if NET8_0_OR_GREATER
	[RequiresUnreferencedCode("Reads the properties of a base type, which the trimmer may remove.")]
#endif
	private static PropertyInfo WithInheritedGetter(PropertyInfo property)
	{
		MethodInfo? definition = property.GetSetMethod(true)?.GetBaseDefinition();
		if (property.CanRead || definition is null || definition.DeclaringType == property.DeclaringType)
		{
			return property;
		}

#pragma warning disable S3011 // https://rules.sonarsource.com/csharp/RSPEC-3011
		return definition.DeclaringType!
			.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			               BindingFlags.DeclaredOnly)
			.FirstOrDefault(declaration => declaration.Name == property.Name &&
			                               declaration.GetIndexParameters().Length == 0) ?? property;
#pragma warning restore S3011
	}

	/// <remarks>
	///     A member is included when it has one of the requested visibilities. Requiring all of them at once would
	///     leave a combination such as <c>Public | Internal</c> without any member.<br />
	///     A <c>protected internal</c> member counts as internal, because the whole assembly can access it, while a
	///     <c>private protected</c> member is only accessible to derived types.
	/// </remarks>
	private static bool Includes(IncludeMembers includeMembers, bool isPublic, bool isAssembly)
		=> (includeMembers.HasFlag(IncludeMembers.Public) && isPublic) ||
		   (includeMembers.HasFlag(IncludeMembers.Internal) && isAssembly);

	/// <remarks>
	///     Reflection returns a member hidden with <see langword="new" /> once per declaration, which makes a lookup
	///     by name ambiguous. Only the declaration on the most derived type takes part, which is also the one a
	///     registration provides.
	/// </remarks>
	private static TMember[] MostDerived<TMember>(IEnumerable<TMember> members)
		where TMember : MemberInfo
	{
		Dictionary<string, TMember> byName = new(StringComparer.Ordinal);
		List<string> names = new();
		foreach (TMember member in members)
		{
			if (!byName.TryGetValue(member.Name, out TMember? existing))
			{
				byName[member.Name] = member;
				names.Add(member.Name);
			}
			else if (existing.DeclaringType != member.DeclaringType &&
			         existing.DeclaringType!.IsAssignableFrom(member.DeclaringType))
			{
				byName[member.Name] = member;
			}
		}

		return names.Select(name => byName[name]).ToArray();
	}
}
