using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have no parameters.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> HaveNoParameters(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterCount(0);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have no parameters.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>>> HaveNoParameters(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterCount(0);
#endif
}
