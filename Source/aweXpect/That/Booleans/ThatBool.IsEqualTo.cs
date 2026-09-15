using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatBool
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<bool, IThat<bool>> IsEqualTo(this IThat<bool> subject,
		bool expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint(it, grammars, expected)),
			subject);

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static AndOrResult<bool, IThat<bool>> IsNotEqualTo(this IThat<bool> subject,
		bool unexpected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint(it, grammars, unexpected).Invert()),
			subject);
}
