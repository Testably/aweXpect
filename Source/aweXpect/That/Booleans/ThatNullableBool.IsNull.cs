using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableBool
{
	/// <summary>
	///     Verifies that the subject is <see langword="null" />.
	/// </summary>
	public static AndOrResult<bool?, IThat<bool?>> IsNull(this IThat<bool?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint(it, grammars, null)),
			subject);

	/// <summary>
	///     Verifies that the subject is not <see langword="null" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<bool?, IThat<bool?>> IsNotNull(this IThat<bool?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint(it, grammars, null).Invert()),
			subject);
}
