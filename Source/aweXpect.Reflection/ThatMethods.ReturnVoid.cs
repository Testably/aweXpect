using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnVoid(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(void));

	/// <summary>
	///     …return <see langword="void" />.
	/// </summary>
	public static ReturnResult<IEnumerable<MethodInfo?>> ReturnVoid(
		this IEnumerableItems<MethodInfo?> items)
		=> items.Return(typeof(void));

	/// <inheritdoc cref="ReturnVoid(IEnumerableItems{MethodInfo})" />
	public static ReturnResult<TCollection> ReturnVoid<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items)
		where TCollection : struct, IEnumerable<MethodInfo?>
		=> items.Return(typeof(void));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnVoid(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(void));

	/// <inheritdoc cref="ReturnVoid(IEnumerableItems{MethodInfo})" />
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> ReturnVoid(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.Return(typeof(void));
#endif

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturnVoid(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> DoNotReturn(subject, typeof(void));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> DoNotReturnVoid(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> DoNotReturn(subject, typeof(void));
#endif
}
