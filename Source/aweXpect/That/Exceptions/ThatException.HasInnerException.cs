using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public partial class ThatException
{
	/// <summary>
	///     Verifies that the actual exception has an inner exception.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInnerException(
		this IThat<Exception?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the actual exception has an inner exception which satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInnerException(
		this IThat<Exception?> subject,
		Action<IThatSubject<Exception?>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);
}
