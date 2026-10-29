using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, int expected)
		=> new(subject.ExpectAll(new HaveParameterCountExpectation<ConstructorInfo>(expected)), subject);

	/// <summary>
	///     …have the <paramref name="expected" /> number of parameters.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IEnumerableItems<ConstructorInfo?> items, int expected)
		=> new(items.ExpectItems(new HaveParameterCountExpectation<ConstructorInfo>(expected)), items.Subject);

	/// <inheritdoc cref="HaveParameterCount(IEnumerableItems{ConstructorInfo}, int)" />
	public static AndOrResult<TCollection, IThat<TCollection>> HaveParameterCount<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items, int expected)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> new(items.ExpectItems(new HaveParameterCountExpectation<ConstructorInfo>(expected)), items.Subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, int expected)
		=> new(subject.ExpectAll(new HaveParameterCountExpectation<ConstructorInfo>(expected)), subject);

	/// <inheritdoc cref="HaveParameterCount(IEnumerableItems{ConstructorInfo}, int)" />
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IAsyncEnumerableItems<ConstructorInfo?> items, int expected)
		=> new(items.ExpectItems(new HaveParameterCountExpectation<ConstructorInfo>(expected)), items.Subject);
#endif
}
