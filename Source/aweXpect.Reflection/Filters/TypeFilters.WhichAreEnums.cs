using System;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are enums.
	/// </summary>
	public static Filtered.Types WhichAreEnums(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type?.IsEnum == true,
			"which are enums "));

	/// <summary>
	///     Filters for types that are not enums.
	/// </summary>
	public static Filtered.Types WhichAreNotEnums(this Filtered.Types @this)
		=> @this.Which(Filter.Suffix<Type>(
			type => type?.IsEnum != true,
			"which are not enums "));
}
