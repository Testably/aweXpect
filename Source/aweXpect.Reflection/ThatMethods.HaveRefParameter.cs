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
	///     a <see langword="ref" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsRefParameter(), "a ref");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsRefParameter(), "a ref");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif

	/// <summary>
	///     …have a <see langword="ref" /> parameter.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<MethodInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IEnumerableItems<MethodInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     …have a <see langword="ref" /> parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveRefParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> HaveParameterOfKind(items, p => p.IsRefParameter(), "a ref");

	/// <inheritdoc cref="HaveRefParameter{TParameter}(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.HaveParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter{TParameter}(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameter<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameter(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly{TParameter}(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType)
		=> items.HaveParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly{TParameter}(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveRefParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
		=> items.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <inheritdoc cref="HaveRefParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveRefParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
		=> items.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
#endif
}
