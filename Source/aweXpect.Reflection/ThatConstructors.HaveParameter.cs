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
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     …have a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, string expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParameter{TParameter}(IEnumerableItems{ConstructorInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter{TParameter}(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> HaveParameter<TParameter>(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{ConstructorInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<ConstructorInfo?> items, string expected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, Type parameterType, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfType(parameterType);
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(typeof(TParameter), unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Type parameterType, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(parameterType, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.Named(null, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	private static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IThat<IEnumerable<ConstructorInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfKind(predicate, kind);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfKind(predicate, kind);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

	private static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, IEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IEnumerableItems<ConstructorInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	private static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameterOfKind<TCollection>(
		IStructEnumerableItems<TCollection, ConstructorInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>?, object?> HaveParameterOfKind(
		IAsyncEnumerableItems<ConstructorInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<ConstructorInfo> expectation = HaveParameterExpectation<ConstructorInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif
}
