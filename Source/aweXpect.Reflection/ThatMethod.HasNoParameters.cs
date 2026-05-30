using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has no parameters.
	/// </summary>
	public static AndOrResult<MethodInfo?, IThat<MethodInfo?>> HasNoParameters(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameterCount(0);
}
