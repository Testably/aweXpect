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
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter), true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter), true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected, true);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     …have a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter), true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of exact type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter of exact type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameterExactly<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter), true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameterExactly{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameterExactly<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameterExactly(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterExactly(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif
}
