using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Equivalency;

internal static class IncludeMembersExtensions
{
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
			yield break;
		}

		BindingFlags bindingFlags = GetBindingFlags(includeMembers);
		foreach (FieldInfo field in type.GetFields(bindingFlags))
		{
			if (!Includes(includeMembers, field.IsPublic, field.IsAssembly, field.IsPrivate))
			{
				continue;
			}

			yield return field;
		}
	}

	public static IEnumerable<PropertyInfo> GetProperties(this Type type, IncludeMembers includeMembers)
	{
		if (includeMembers == IncludeMembers.None)
		{
			yield break;
		}

		BindingFlags bindingFlags = GetBindingFlags(includeMembers);
		foreach (PropertyInfo property in type.GetProperties(bindingFlags).Where(x => x.CanRead))
		{
			MethodInfo getter = property.GetAccessors(true)[0];
			if (!getter.Name.StartsWith("get_", StringComparison.Ordinal) ||
			    !Includes(includeMembers, getter.IsPublic, getter.IsAssembly, getter.IsPrivate))
			{
				continue;
			}

			yield return property;
		}
	}

	/// <remarks>
	///     A member is included when it has one of the requested visibilities. Requiring all of them at once would
	///     leave a combination such as <c>Public | Internal</c> without any member.
	/// </remarks>
	private static bool Includes(IncludeMembers includeMembers, bool isPublic, bool isAssembly, bool isPrivate)
		=> (includeMembers.HasFlag(IncludeMembers.Public) && isPublic) ||
		   (includeMembers.HasFlag(IncludeMembers.Internal) && isAssembly) ||
		   (includeMembers.HasFlag(IncludeMembers.Private) && isPrivate);
}
