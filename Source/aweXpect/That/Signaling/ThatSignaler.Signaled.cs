using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect;

public static partial class ThatSignaler
{
	private const string Times = " times";

	/// <summary>
	///     The occurrences of a <see cref="Quantifier" /> that was never further specified.
	/// </summary>
	/// <remarks>
	///     They are left out of the negated expectation, so that <c>DidNotSignal()</c> keeps reading as
	///     "does not have recorded the callback" instead of adding a redundant "at least once".
	/// </remarks>
	private static readonly string DefaultOccurrences = new Quantifier().ToString();

	/// <summary>
	///     Verifies that the expected callback was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions options = new();
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions<TParameter> options = new();
		return new SignalCountWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions options = new();
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions<TParameter> options = new();
		return new SignalCountWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult DidNotSignal(
		this IThat<Signaler> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions options = new();
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions<TParameter> options = new();
		return new SignalCountResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult DidNotSignal(
		this IThat<Signaler> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions options = new();
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions<TParameter> options = new();
		return new SignalCountResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options).Invert()),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     The number of signals after which the <paramref name="quantifier" /> decides without knowing whether more follow.
	/// </summary>
	/// <remarks>
	///     Waiting for exactly this many signals lets the quantifiers that can be satisfied early (e.g. <c>AtLeast</c>)
	///     short-circuit, while all others (e.g. <c>AtMost</c>) wait out the timeout, because only then is the recorded
	///     count final.
	/// </remarks>
	private static int GetDecisiveCount(Quantifier quantifier)
	{
		int count = 0;
		while (quantifier.Check(count, false) is null)
		{
			count++;
		}

		return count;
	}

	private static void AppendNormalCallbackExpectation(StringBuilder stringBuilder, Quantifier quantifier,
		SignalerOptions options)
	{
		if (quantifier.IsNever)
		{
			stringBuilder.Append("has never recorded the callback");
		}
		else
		{
			stringBuilder.Append("has recorded the callback ").Append(quantifier);
		}

		stringBuilder.Append(options);
	}

	private static void AppendNegatedCallbackExpectation(StringBuilder stringBuilder, Quantifier quantifier,
		SignalerOptions options)
	{
		stringBuilder.Append("does not have recorded the callback");
		string occurrences = quantifier.ToString();
		if (occurrences != DefaultOccurrences)
		{
			stringBuilder.Append(' ').Append(occurrences);
		}

		stringBuilder.Append(options);
	}

	private static void AppendOccurrences(StringBuilder stringBuilder, Quantifier quantifier, int count)
	{
		if (count == 0)
		{
			stringBuilder.Append("never recorded");
			return;
		}

		if (quantifier.Check(count, false) is null)
		{
			stringBuilder.Append("only ");
		}

		stringBuilder.Append("recorded ");
		if (count == 1)
		{
			stringBuilder.Append("once");
		}
		else if (count == 2)
		{
			stringBuilder.Append("twice");
		}
		else
		{
			stringBuilder.Append(count).Append(Times);
		}
	}

	private sealed class SignaledConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		SignalerOptions options)
		: ConstraintResult.WithNotNullValue<SignalerResult>(it, grammars), IAsyncConstraint<Signaler>
	{
		public async Task<ConstraintResult> IsMetBy(Signaler actual, CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			int decisiveCount = GetDecisiveCount(quantifier);
			TimeSpan? timeout = decisiveCount > 0 ? options.Timeout : TimeSpan.Zero;
			int amount = Math.Max(1, decisiveCount);
			Actual = await Task.Run(()
					=> actual.Wait(amount.Times(), timeout, cancellationToken),
				CancellationToken.None);

			Outcome = quantifier.Check(Actual.Count, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalCallbackExpectation(stringBuilder, quantifier, options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, quantifier, Actual?.Count ?? 0);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNegatedCallbackExpectation(stringBuilder, quantifier, options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class SignaledConstraint<TParameter>(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		SignalerOptions<TParameter> options)
		: ConstraintResult.WithNotNullValue<SignalerResult<TParameter>>(it, grammars),
			IAsyncConstraint<Signaler<TParameter>>
	{
		private int _actualCount;

		public async Task<ConstraintResult> IsMetBy(
			Signaler<TParameter> actual,
			CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			SignalerOptions<TParameter> o = options;
			int decisiveCount = GetDecisiveCount(quantifier);
			TimeSpan? timeout = decisiveCount > 0 ? options.Timeout : TimeSpan.Zero;
			int amount = Math.Max(1, decisiveCount);
			Actual = await Task.Run(()
					=> actual.Wait(amount.Times(), o.Matches, timeout, cancellationToken),
				CancellationToken.None);

			_actualCount = Actual.Parameters.Count(p => o.Matches(p));

			Outcome = quantifier.Check(_actualCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalCallbackExpectation(stringBuilder, quantifier, options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, quantifier, _actualCount);

			if (Actual?.Count > 0)
			{
				stringBuilder.Append(" in ");
				ValueFormatters.Format(Formatter, stringBuilder, Actual.Parameters, FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNegatedCallbackExpectation(stringBuilder, quantifier, options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
