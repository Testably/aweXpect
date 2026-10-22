using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the method returns <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static MethodReturnResult<MethodInfo, IThat<MethodInfo?>> ReturnsVoid(
		this IThat<MethodInfo?> subject)
		=> Returns(subject, typeof(void));

	/// <summary>
	///     Verifies that the method does not return <see langword="void" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<MethodInfo, IThat<MethodInfo?>> DoesNotReturnVoid(
		this IThat<MethodInfo?> subject)
		=> DoesNotReturn(subject, typeof(void));
}
