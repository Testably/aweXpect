using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatBool
{
	/// <summary>
	///     Verifies that the subject is <see langword="false" />.
	/// </summary>
	public static AndOrResult<bool, IThat<bool>> IsFalse(this IThat<bool> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint(it, grammars, false)),
			source);
}
