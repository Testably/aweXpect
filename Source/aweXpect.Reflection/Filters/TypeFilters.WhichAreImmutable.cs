using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that are immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static Filtered.Types WhichAreImmutable(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => type.IsImmutable(),
			"immutable "));

	/// <summary>
	///     Filters for types that are not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static Filtered.Types WhichAreNotImmutable(this Filtered.Types @this)
		=> @this.Which(Filter.Prefix<Type>(
			type => !type.IsImmutable(),
			"mutable "));
}
