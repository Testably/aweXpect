using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Excludes constructors that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all constructors matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Constructors Except(this Filtered.Constructors @this,
		Func<ConstructorInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<ConstructorInfo>(
			constructor => !UserCode.Invoke(predicate, constructor, "the predicate"),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
