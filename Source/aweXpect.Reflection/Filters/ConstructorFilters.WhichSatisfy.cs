using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filters for constructors that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Constructors WhichSatisfy(this Filtered.Constructors @this,
		Func<ConstructorInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue} "));
}
