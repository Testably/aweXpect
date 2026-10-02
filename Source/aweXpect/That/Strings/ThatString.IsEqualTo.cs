using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsEqualTo(
		this IThat<string?> subject,
		string? expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new IsEqualToConstraint(expectationBuilder, it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsNotEqualTo(
		this IThat<string?> subject,
		string? unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new IsEqualToConstraint(expectationBuilder, it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithEqualToValue<string?>(it, grammars, expected is null),
			IAsyncConstraint<string?>
	{
		private bool _addsContextWhenNegated;

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		/// <remarks>
		///     A match type that inspects the content of the subject, e.g. a prefix or a pattern, fails for a
		///     <see langword="null" /> subject in both polarities, because it has no content.
		/// </remarks>
		public override Outcome Outcome
		{
			get => Actual is null && options.InspectsSubject
				? Outcome.Failure
				: base.Outcome;
			protected set => base.Outcome = value;
		}

		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = await options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			_addsContextWhenNegated = Outcome == Outcome.Success && !string.IsNullOrEmpty(actual);
			if (!string.IsNullOrEmpty(actual))
			{
				expectationBuilder.AddStringContext("Actual", actual, this);

				if (Outcome != Outcome.Success)
				{
					AddExpectedContext(false);
				}
			}

			return this;
		}

		public override ConstraintResult Negate()
		{
			base.Negate();
			// A negation after the evaluation (e.g. by `DoesNotComplyWith`) turns the success into a failure.
			// Only the first one adds the context, as the next one reverts it before a repeated evaluation.
			if (_addsContextWhenNegated)
			{
				_addsContextWhenNegated = false;
				AddExpectedContext(true);
			}

			return this;
		}

		private void AddExpectedContext(bool onlyOnFailure)
		{
			if (!string.IsNullOrEmpty(expected))
			{
				expectationBuilder.AddStringContext("Expected", expected, this, onlyOnFailure);
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual, expected)
				.Indent(indentation, false));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.GetExtendedFailure(It, Grammars, Actual, expected)
				.Indent(indentation, false));
	}
}
