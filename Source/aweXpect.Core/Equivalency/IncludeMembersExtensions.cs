using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Equivalency;

internal static class IncludeMembersExtensions
{
	/// <remarks>
	///     The members of a type are looked up once per member of the compared object, so both the readable members
	///     and the visibility-filtered result are kept per type.
	/// </remarks>
	private static readonly ConcurrentDictionary<(Type, BindingFlags), FieldInfo[]> AllFields = new();

	private static readonly ConcurrentDictionary<(Type, BindingFlags), PropertyInfo[]> AllProperties = new();
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
				.Where(field => Includes(key.Item2, field.IsPublic, field.IsAssembly, field.IsPrivate))
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
					return Includes(key.Item2, getter.IsPublic, getter.IsAssembly, getter.IsPrivate);
				})
				.ToArray());
	}

	/// <summary>
	///     Finds the field <paramref name="name" /> the way a lookup with the binding flags of
	///     <paramref name="includeMembers" /> does, without requiring the field itself to have a requested visibility.
	/// </summary>
	public static FieldInfo? FindField(this Type type, string name, IncludeMembers includeMembers)
		=> GetAllFields(type, includeMembers).FirstOrDefault(field => field.Name == name);

	/// <summary>
	///     Finds the property <paramref name="name" /> the way a lookup with the binding flags of
	///     <paramref name="includeMembers" /> does, without requiring the property itself to have a requested
	///     visibility.
	/// </summary>
	/// <remarks>
	///     A public request only sees a property that can be read publicly, because a registration cannot call a
	///     non-public getter and the two paths have to agree.
	/// </remarks>
	public static PropertyInfo? FindProperty(this Type type, string name, IncludeMembers includeMembers)
		=> GetAllProperties(type, includeMembers).FirstOrDefault(property
			=> property.Name == name &&
			   (includeMembers != IncludeMembers.Public || property.GetGetMethod(true)!.IsPublic));

	private static FieldInfo[] GetAllFields(Type type, IncludeMembers includeMembers)
		=> AllFields.GetOrAdd((type, GetBindingFlags(includeMembers)), static key
			=> MostDerived(key.Item1.GetFields(key.Item2)));

	/// <remarks>
	///     An indexer is a property whose getter takes arguments, so its value cannot be read for the comparison.
	/// </remarks>
	private static PropertyInfo[] GetAllProperties(Type type, IncludeMembers includeMembers)
		=> AllProperties.GetOrAdd((type, GetBindingFlags(includeMembers)), static key
			=> MostDerived(key.Item1.GetProperties(key.Item2)
				.Where(property => property.CanRead && property.GetIndexParameters().Length == 0)));

	/// <remarks>
	///     A member is included when it has one of the requested visibilities. Requiring all of them at once would
	///     leave a combination such as <c>Public | Internal</c> without any member.
	/// </remarks>
	private static bool Includes(IncludeMembers includeMembers, bool isPublic, bool isAssembly, bool isPrivate)
		=> (includeMembers.HasFlag(IncludeMembers.Public) && isPublic) ||
		   (includeMembers.HasFlag(IncludeMembers.Internal) && isAssembly) ||
		   (includeMembers.HasFlag(IncludeMembers.Private) && isPrivate);

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
