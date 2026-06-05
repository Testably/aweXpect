using System;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Excludes types that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all types matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Types Except(this Filtered.Types @this,
		Func<Type, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<Type>(
			type => !predicate(type),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));

	/// <summary>
	///     Excludes the type <typeparamref name="T" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all types matching a rule except <typeparamref name="T" />.
	/// </remarks>
	public static Filtered.Types Except<T>(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type != typeof(T),
			$"except {Formatter.Format(typeof(T))} "));
}
