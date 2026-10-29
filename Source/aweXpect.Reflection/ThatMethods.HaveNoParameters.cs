using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have no parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> HaveNoParameters(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterCount(0);

	/// <summary>
	///     …have no parameters.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> HaveNoParameters(
		this IEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterCount(0);

	/// <inheritdoc cref="HaveNoParameters(IEnumerableItems{MethodInfo})" />
	public static AndOrResult<TCollection, IThat<TCollection>> HaveNoParameters<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.HaveParameterCount(0);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have no parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> HaveNoParameters(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterCount(0);

	/// <inheritdoc cref="HaveNoParameters(IEnumerableItems{MethodInfo})" />
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> HaveNoParameters(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.HaveParameterCount(0);
#endif
}
