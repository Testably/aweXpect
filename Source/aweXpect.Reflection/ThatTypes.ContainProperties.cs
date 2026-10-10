using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain properties matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching property. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited properties are considered.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainProperties(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Properties, Filtered.Properties> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<PropertyInfo, Filtered.Properties>(subject, types => types.Properties(memberScope), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain properties matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching property. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited properties are considered.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainProperties(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Properties, Filtered.Properties> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<PropertyInfo, Filtered.Properties>(subject, types => types.Properties(memberScope), filter);
#endif
}
