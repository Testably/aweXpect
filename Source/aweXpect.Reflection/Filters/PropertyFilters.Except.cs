using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class PropertyFilters
{
	/// <summary>
	///     Excludes properties that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all properties matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Properties Except(this Filtered.Properties @this,
		Func<PropertyInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<PropertyInfo>(
			property => !predicate(property),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
