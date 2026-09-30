using System.Text;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class MyRepeatedCheckExtensions
{
	public static RepeatedCheckResult<Probe, IThat<Probe>> DoesNotReturnPositive(this IThat<Probe> subject)
	{
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<Probe, IThat<Probe>>(((IExpectThat<Probe>)subject).ExpectationBuilder
				.AddConstraint((expectationBuilder, it, grammars)
					=> new ReturnsPositiveConstraint(expectationBuilder, it, grammars, options).Invert()),
			subject,
			options);
	}

	public static RepeatedCheckResult<Probe, IThat<Probe>> ReturnsPositive(this IThat<Probe> subject)
	{
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<Probe, IThat<Probe>>(((IExpectThat<Probe>)subject).ExpectationBuilder
				.AddConstraint((expectationBuilder, it, grammars)
					=> new ReturnsPositiveConstraint(expectationBuilder, it, grammars, options)),
			subject,
			options);
	}

	public sealed class Probe(Func<int> read)
	{
		public int Read() => read();
	}

	private sealed class ReturnsPositiveConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		RepeatedCheckOptions options)
		: ConstraintResult.WithNotNullValue<Probe>(it, grammars),
			IAsyncConstraint<Probe>
	{
		private int _returned;

		public async Task<ConstraintResult> IsMetBy(Probe actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			await options.CheckRepeatedly(() =>
			{
				_returned = actual.Read();
				Outcome = _returned > 0 ? Outcome.Success : Outcome.Failure;
				return Task.FromResult(_returned > 0 != IsNegated);
			}, expectationBuilder, cancellationToken);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("returns a positive value").Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" returned ").Append(_returned);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not return a positive value").Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
