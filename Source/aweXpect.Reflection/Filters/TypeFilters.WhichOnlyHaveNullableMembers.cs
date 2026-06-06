using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types whose fields, properties and events are all nullable, including inherited members or only
	///     those declared directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	public static Filtered.Types WhichOnlyHaveNullableMembers(this Filtered.Types @this,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.GetNotNullableMembers(memberScope).Length == 0,
			"which only have nullable members "));

	/// <summary>
	///     Filters for types whose fields, properties and events are all non-nullable, including inherited members or
	///     only those declared directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	public static Filtered.Types WhichOnlyHaveNonNullableMembers(this Filtered.Types @this,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> @this.Which(Filter.Suffix<Type>(
			type => type.GetNullableMembers(memberScope).Length == 0,
			"which only have non-nullable members "));
}
