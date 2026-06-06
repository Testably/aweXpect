using System.Reflection;
using aweXpect.Core;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns <see langword="void" />.
	/// </summary>
	public static MethodReturnResult<MethodInfo?, IThat<MethodInfo?>> ReturnsVoid(
		this IThat<MethodInfo?> subject)
		=> Returns(subject, typeof(void));
}
