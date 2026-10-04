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
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars) =>
					new EndsWithConstraint(it, grammars, state.Expected, state.Options)),
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
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars) =>
					new EndsWithConstraint(it, grammars, state.Unexpected, state.Options).Invert()),
			subject,
			options);
	}

	private sealed class EndsWithConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		public async ValueTask<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = await options.AreConsideredEqual(actual, expected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(Actual))
			{
				contexts.AddStringContext("Actual", Actual, this);
				contexts.AddStringContext("Expected", expected, this);
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
