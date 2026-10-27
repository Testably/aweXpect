using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types which have an implicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveImplicitConversionOperator<TSource, TTarget>(
		this Filtered.Types @this)
		=> @this.WhichHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Filter for types which have an implicit conversion operator from <paramref name="source" /> to
	///     <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveImplicitConversionOperator(this Filtered.Types @this,
		Type source, Type target)
		=> new(@this, memberScope => ConversionOperatorFilter(true, source, target, memberScope.IsDirect(), true));

	/// <summary>
	///     Filter for types which do not have an implicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveImplicitConversionOperator<TSource, TTarget>(
		this Filtered.Types @this)
		=> @this.WhichDoNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Filter for types which do not have an implicit conversion operator from <paramref name="source" /> to
	///     <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveImplicitConversionOperator(this Filtered.Types @this,
		Type source, Type target)
		=> new(@this, memberScope => ConversionOperatorFilter(true, source, target, memberScope.IsDirect(), false));

	/// <summary>
	///     Filter for types which have an explicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion, which C# requires alongside a <c>checked</c> one.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveExplicitConversionOperator<TSource, TTarget>(
		this Filtered.Types @this)
		=> @this.WhichHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Filter for types which have an explicit conversion operator from <paramref name="source" /> to
	///     <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion, which C# requires alongside a <c>checked</c> one.
	/// </remarks>
	public static InheritanceScopeResult WhichHaveExplicitConversionOperator(this Filtered.Types @this,
		Type source, Type target)
		=> new(@this, memberScope => ConversionOperatorFilter(false, source, target, memberScope.IsDirect(), true));

	/// <summary>
	///     Filter for types which do not have an explicit conversion operator from <typeparamref name="TSource" /> to
	///     <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion, which C# requires alongside a <c>checked</c> one.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveExplicitConversionOperator<TSource, TTarget>(
		this Filtered.Types @this)
		=> @this.WhichDoNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Filter for types which do not have an explicit conversion operator from <paramref name="source" /> to
	///     <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the filter to the operators declared
	///     on the type with <see cref="InheritanceScopeResult.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion, which C# requires alongside a <c>checked</c> one.
	/// </remarks>
	public static InheritanceScopeResult WhichDoNotHaveExplicitConversionOperator(this Filtered.Types @this,
		Type source, Type target)
		=> new(@this, memberScope => ConversionOperatorFilter(false, source, target, memberScope.IsDirect(), false));

	private static Filter.GenericFilter<Type> ConversionOperatorFilter(bool isImplicit, Type source, Type target, bool directly,
		bool expected)
		=> Filter.Suffix<Type>(
			type => (type.GetConversionOperator(isImplicit, source, target, !directly) is not null) == expected,
			() => $"which {(expected ? "" : "do not ")}{(directly ? "declare" : "have")} an {(isImplicit ? "implicit" : "explicit")} conversion operator from {Formatter.Format(source)} to {Formatter.Format(target)} ");
}
