using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has no parameters.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> HasNoParameters(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameterCount(0);
}
