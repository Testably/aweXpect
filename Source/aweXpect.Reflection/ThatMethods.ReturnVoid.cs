using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IEnumerable<MethodInfo?>?, IThat<IEnumerable<MethodInfo?>?>> ReturnVoid(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(void));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo?>?, IThat<IAsyncEnumerable<MethodInfo?>?>> ReturnVoid(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(void));
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
