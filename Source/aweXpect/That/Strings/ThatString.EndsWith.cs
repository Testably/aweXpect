using System;
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
	///     Verifies that the subject ends with the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<string, IThat<string?>> EndsWith(
		this IThat<string?> subject,
		string expected)
	{
		expected.ThrowIfNull();
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' string cannot be empty.", nameof(expected)));
		}

		StringEqualityOptions options = new StringEqualityOptions(nameof(expected)).AsSuffix();
		return new StringEqualityResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new EndsWithConstraint(expectationBuilder, it, grammars, expected, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not end with the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<string, IThat<string?>> DoesNotEndWith(
		this IThat<string?> subject,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected)));
		}

		StringEqualityOptions options = new StringEqualityOptions(nameof(unexpected)).AsSuffix();
		return new StringEqualityResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new EndsWithConstraint(expectationBuilder, it, grammars, unexpected, options).Invert()),
			subject,
			options);
	}

	private sealed class EndsWithConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		private bool _addsContextWhenNegated;

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
