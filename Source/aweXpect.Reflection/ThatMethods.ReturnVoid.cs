using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	public static MethodsReturnResult<IEnumerable<MethodInfo>, IThat<IEnumerable<MethodInfo>>> ReturnVoid(
		this IThat<IEnumerable<MethodInfo>> subject)
		=> Return(subject, typeof(void));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return <see langword="void" />.
	/// </summary>
	public static MethodsReturnResult<IAsyncEnumerable<MethodInfo>, IThat<IAsyncEnumerable<MethodInfo>>> ReturnVoid(
		this IThat<IAsyncEnumerable<MethodInfo>> subject)
		=> Return(subject, typeof(void));
#endif
}
