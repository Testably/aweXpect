using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> ReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IEnumerableItems<MethodInfo?> items)
		=> items.ReturnExactly(typeof(TReturn));

	/// <summary>
	///     …return exactly type <paramref name="returnType" />.
	/// </summary>
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnExactly(
		this IEnumerableItems<MethodInfo?> items, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="ReturnExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ReturnResult<TCollection> ReturnExactly<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type returnType)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		ReturnExpectation expectation = new(returnType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> ReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, true);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="ReturnExactly{TReturn}(IEnumerableItems{MethodInfo})" />
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly<TReturn>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.ReturnExactly(typeof(TReturn));

	/// <inheritdoc cref="ReturnExactly(IEnumerableItems{MethodInfo}, Type)" />
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnExactly(
		this IAsyncEnumerableItems<MethodInfo?> items, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturnExactly<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> DoNotReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturnExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
		=> new(subject.ExpectNone(new ReturnExpectation(returnType, true)), subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>>
		DoNotReturnExactly<TReturn>(
			this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> DoNotReturnExactly(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return exactly type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> DoNotReturnExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
		=> new(subject.ExpectNone(new ReturnExpectation(returnType, true)), subject);
#endif
}
