using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Equivalency;

internal static class IncludeMembersExtensions
{
	/// <remarks>
	///     The members of a type are looked up once per member of the compared object, so the filtered result is
	///     kept per type and option.
	/// </remarks>
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
			=> MostDerived(key.Item1.GetFields(GetBindingFlags(key.Item2))
				.Where(field => Includes(key.Item2, field.IsPublic, field.IsAssembly, field.IsPrivate))));
	}

	/// <remarks>
	///     An indexer is a property whose getter takes arguments, so its value cannot be read for the comparison.
	/// </remarks>
	public static IEnumerable<PropertyInfo> GetProperties(this Type type, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None)
		{
			return Enumerable.Empty<PropertyInfo>();
		}

		return Properties.GetOrAdd((type, includeMembers), static key
			=> MostDerived(key.Item1.GetProperties(GetBindingFlags(key.Item2))
				.Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
				.Where(property =>
				{
					MethodInfo getter = property.GetGetMethod(true)!;
					return Includes(key.Item2, getter.IsPublic, getter.IsAssembly, getter.IsPrivate);
				})));
	}

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
