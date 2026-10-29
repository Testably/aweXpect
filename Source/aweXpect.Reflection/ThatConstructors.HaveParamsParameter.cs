using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsParamsParameter(), "a params");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsParamsParameter(), "a params");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif

	/// <summary>
	///     …have a <see langword="params" /> parameter.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsParamsParameter(), "a params");

	/// <summary>
	///     …have a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     …have a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> HaveParameterOfKind(items, p => p.IsParamsParameter(), "a params");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParamsParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsParamsParameter(), "a params");

	/// <inheritdoc cref="HaveParamsParameter{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <inheritdoc cref="HaveParamsParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParamsParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif
}
