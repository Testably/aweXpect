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
	///     Verifies that the subject is equal to <paramref name="expected" />.
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
	///     Verifies that the subject is not equal to <paramref name="unexpected" />.
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
		/// <inheritdoc cref="ConstraintResult.Outcome" />
		/// <remarks>
		///     A match type other than the exact one inspects the content of the subject, which a <see langword="null" />
		///     does not have, so it fails in both polarities. A <see langword="null" /> pattern inspects nothing and
		///     degenerates to a plain equality check, for which <see langword="null" /> is a legitimate answer.
		/// </remarks>
		public override Outcome Outcome
		{
			get => Actual is null && expected is not null && options.InspectsSubject
				? Outcome.Failure
				: base.Outcome;
			protected set => base.Outcome = value;
		}

		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = await options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			if (!string.IsNullOrEmpty(actual))
			{
				expectationBuilder.AddStringContext("Actual", actual, this);

				if (Outcome != Outcome.Success && !string.IsNullOrEmpty(expected))
				{
					expectationBuilder.AddStringContext("Expected", expected, this);
				}
			}

			return this;
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
