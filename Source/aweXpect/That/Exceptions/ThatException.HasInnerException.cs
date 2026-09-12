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
		this IThat<Exception?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			source);

	/// <summary>
	///     Verifies that the actual exception has an inner exception which satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Exception?, IThat<Exception?>> HasInnerException(
		this IThat<Exception?> source,
		Action<IThatSubject<Exception?>> expectations)
		=> new(source.Get().ExpectationBuilder
				.ForMember<Exception, Exception?>(e => e.InnerException,
					" whose ",
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			source);
}
