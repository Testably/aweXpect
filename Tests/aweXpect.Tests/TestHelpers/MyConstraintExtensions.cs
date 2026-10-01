using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class MyConstraintExtensions
{
	public static AndOrResult<bool, IThat<bool>> IsMyConstraint(this IThat<bool> subject,
		string expectation,
		Func<bool, bool> isSuccess, string failureMessage)
		=> new(((IExpectThat<bool>)subject).ExpectationBuilder.AddConstraint((_, grammars)
				=> new MyConstraint(grammars, expectation, isSuccess, failureMessage)),
			subject);

	private sealed class MyConstraint(
		ExpectationGrammars grammars,
		string expectation,
		Func<bool, bool> isSuccess,
		string failureMessage)
		: ConstraintResult(grammars), IValueConstraint<bool>
	{
		public ConstraintResult IsMetBy(bool actual)
		{
			Outcome = isSuccess(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation.Indent(indentation, false));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(failureMessage.Indent(indentation, false));

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
			=> this;
	}
}
