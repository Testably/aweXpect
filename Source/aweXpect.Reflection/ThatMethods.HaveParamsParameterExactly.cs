using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParamsParameterExactly{TParameter}(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly{TParameter}(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif
}
