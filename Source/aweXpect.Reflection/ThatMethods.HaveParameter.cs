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
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, expected);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     …have a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IEnumerableItems<MethodInfo?> items)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IEnumerableItems<MethodInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     …have a parameter of type <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter of type <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     …have a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type parameterType, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<TCollection, TCollection, object?> HaveParameter<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, string expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveParameter{TParameter}(IEnumerableItems{MethodInfo})" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, Type)" />
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <inheritdoc cref="HaveParameter{TParameter}(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParameter<TParameter>(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, Type, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, Type parameterType, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <inheritdoc cref="HaveParameter(IEnumerableItems{MethodInfo}, string)" />
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameter(
		this IAsyncEnumerableItems<MethodInfo?> items, string expected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, expected);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(typeof(TParameter));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfType(parameterType);
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <typeparamref name="TParameter" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> DoNotHaveParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(typeof(TParameter), unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter of type <paramref name="parameterType" /> with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(parameterType, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> DoNotHaveParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string unexpected)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.Named(null, unexpected, parameterName: nameof(unexpected));
		return new(subject.ExpectNone(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter, expectation.NameEquality);
	}
#endif

	private static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IThat<IEnumerable<MethodInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfKind(predicate, kind);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IThat<IAsyncEnumerable<MethodInfo?>?> subject, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfKind(predicate, kind);
		return new(subject.ExpectAll(expectation), subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif

	private static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IEnumerableItems<MethodInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

	private static ParameterCollectionResult<TCollection, TCollection, object?> HaveParameterOfKind<TCollection>(
		IStructEnumerableItems<TCollection, MethodInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}

#if NET8_0_OR_GREATER
	private static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParameterOfKind(
		IAsyncEnumerableItems<MethodInfo?> items, Func<ParameterInfo, bool> predicate, string kind)
	{
		HaveParameterExpectation<MethodInfo> expectation = HaveParameterExpectation<MethodInfo>.OfKind(predicate, kind);
		return new(items.ExpectItems(expectation), items.Subject, expectation.CollectionIndex, expectation.ParameterFilter);
	}
#endif
}
