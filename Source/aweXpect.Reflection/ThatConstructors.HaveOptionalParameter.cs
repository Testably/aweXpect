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
	///     an optional parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsOptionalParameter(), "an optional");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsOptionalParameter(), "an optional");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an optional parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");
#endif

	/// <summary>
	///     …have an optional parameter.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsOptionalParameter(), "an optional");

	/// <summary>
	///     …have an optional parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     …have an optional parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     …have an optional parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     …have an optional parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <summary>
	///     …have an optional parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveOptionalParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> HaveParameterOfKind(items, p => p.IsOptionalParameter(), "an optional");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveOptionalParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveOptionalParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveOptionalParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsOptionalParameter(), "an optional");

	/// <inheritdoc cref="HaveOptionalParameter{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveOptionalParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");

	/// <inheritdoc cref="HaveOptionalParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveOptionalParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsOptionalParameter(), "with optional modifier");
#endif
}
