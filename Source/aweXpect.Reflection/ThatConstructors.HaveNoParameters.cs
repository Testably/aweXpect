using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have no parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> HaveNoParameters(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterCount(0);

	/// <summary>
	///     …have no parameters.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> HaveNoParameters(
		this IEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterCount(0);

	/// <inheritdoc cref="HaveNoParameters(IEnumerableItems{ConstructorInfo})" />
	public static AndOrResult<TCollection, IThat<TCollection>> HaveNoParameters<TCollection>(
		this IStructEnumerableItems<TCollection, ConstructorInfo?> items)
		where TCollection : struct, IEnumerable<ConstructorInfo?>
		=> items.HaveParameterCount(0);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have no parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> HaveNoParameters(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		=> subject.HaveParameterCount(0);

	/// <inheritdoc cref="HaveNoParameters(IEnumerableItems{ConstructorInfo})" />
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> HaveNoParameters(
		this IAsyncEnumerableItems<ConstructorInfo?> items)
		=> items.HaveParameterCount(0);
#endif
}
