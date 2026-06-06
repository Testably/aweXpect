using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class FieldFilters
{
	/// <summary>
	///     Excludes fields that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all fields matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Fields Except(this Filtered.Fields @this,
		Func<FieldInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<FieldInfo>(
			field => !predicate(field),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
