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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain constructors matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching constructor. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypeContainingMembersResult<IEnumerable<Type?>> ContainConstructors(
		this IThat<IEnumerable<Type?>> subject,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
		=> Contain<ConstructorInfo, Filtered.Constructors>(subject, types => types.Constructors(), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain constructors matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching constructor. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>> ContainConstructors(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Func<Filtered.Constructors, Filtered.Constructors> filter)
		=> Contain<ConstructorInfo, Filtered.Constructors>(subject, types => types.Constructors(), filter);
#endif
}
