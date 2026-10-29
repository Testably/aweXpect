using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IThat<IEnumerable<MethodInfo?>?> subject, int expected)
		=> new(subject.ExpectAll(new HaveParameterCountExpectation<MethodInfo>(expected)), subject);

	/// <summary>
	///     …have the <paramref name="expected" /> number of parameters.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IEnumerableItems<MethodInfo?> items, int expected)
		=> new(items.ExpectItems(new HaveParameterCountExpectation<MethodInfo>(expected)), items.Subject);

	/// <inheritdoc cref="HaveParameterCount(IEnumerableItems{MethodInfo}, int)" />
	public static AndOrResult<TCollection, IThat<TCollection>> HaveParameterCount<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, int expected)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> new(items.ExpectItems(new HaveParameterCountExpectation<MethodInfo>(expected)), items.Subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, int expected)
		=> new(subject.ExpectAll(new HaveParameterCountExpectation<MethodInfo>(expected)), subject);

	/// <inheritdoc cref="HaveParameterCount(IEnumerableItems{MethodInfo}, int)" />
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IAsyncEnumerableItems<MethodInfo?> items, int expected)
		=> new(items.ExpectItems(new HaveParameterCountExpectation<MethodInfo>(expected)), items.Subject);
#endif
}
