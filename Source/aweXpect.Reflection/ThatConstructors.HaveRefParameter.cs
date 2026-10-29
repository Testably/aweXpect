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
	///     a <see langword="ref" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsRefParameter(), "a ref");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsRefParameter(), "a ref");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif

	/// <summary>
	///     …have a <see langword="ref" /> parameter.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <inheritdoc cref="HaveRefParameter{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveRefParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif
}
