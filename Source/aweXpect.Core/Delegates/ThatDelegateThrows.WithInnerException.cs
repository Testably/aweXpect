using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the thrown exception has an inner exception which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInnerException(
		Action<IThatSubject<Exception?>> expectations)
		=> new(ExpectationBuilder
				.ForMember(
					MemberAccessor<Exception?, Exception?>.FromFunc(
						e => e?.InnerException,
						"the inner exception"),
					(_, s) => s.Append(" which "),
					false)
				.Validate((it, grammars)
					=> new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<Exception?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			this);

	/// <summary>
	///     Verifies that the actual exception has an inner exception of type <typeparamref name="TException" />.
	/// </summary>
	public AndOrResult<TException, ThatDelegateThrows<TException>> WithInnerException()
		=> new(ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new HasInnerExceptionValueConstraint(typeof(Exception), it, grammars)),
			this);
}
