using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters the types according to the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Types WhichSatisfy(this Filtered.Types types,
		Func<Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> types.Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue} "));
}
