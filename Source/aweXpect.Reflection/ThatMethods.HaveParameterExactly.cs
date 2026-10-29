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
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter), true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter), true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     …have a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter), true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParameterExactly{TParameter}(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter), true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly{TParameter}(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif
}
