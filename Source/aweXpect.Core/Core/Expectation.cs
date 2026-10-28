using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     Base class for expectation results.
/// </summary>
/// <remarks>
///     Create instances by using the static methods on the <see cref="Expect" /> class and the expectation methods.
///     This class is not meant to be derived from directly: extension authors return an
///     <see cref="aweXpect.Results.ExpectationResult" /> or <see cref="aweXpect.Results.ExpectationResult{TType, TSelf}" />
///     for results and derive custom combinations from <see cref="Combination{TSelf}" />.
/// </remarks>
[StackTraceHidden]
public abstract class Expectation
{
	/// <remarks>
	///     Only aweXpect.Core can derive, because the evaluation relies on members that are not public.
	/// </remarks>
	private protected Expectation()
	{
	}

#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
	/// <summary>
	///     <i>Not supported!</i><br />
	///     <see cref="object.Equals(object?)" /> is not supported. Did you mean <c>IsEqualTo</c> instead?
	/// </summary>
	/// <remarks>
	///     Consider adding support for <see cref="EditorBrowsableAttribute" /> to hide this method from code suggestions.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool Equals(object? obj)
		=> throw Tracing.WriteException(new NotSupportedException("Equals is not supported. Did you mean IsEqualTo() instead?"));
#pragma warning restore S3877

#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
	/// <summary>
	///     <i>Not supported!</i><br />
	///     <see cref="object.GetHashCode()" /> is not supported.
	/// </summary>
	/// <remarks>
	///     Consider adding support for <see cref="EditorBrowsableAttribute" /> to hide this method from code suggestions.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int GetHashCode()
		=> throw Tracing.WriteException(new NotSupportedException("GetHashCode is not supported."));
#pragma warning restore S3877

	/// <summary>
	///     <i>Not supported!</i><br />
	///     <see cref="object.GetType()" /> is not supported.
	/// </summary>
	/// <remarks>
	///     Consider adding support for <see cref="EditorBrowsableAttribute" /> to hide this method from code suggestions.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Type GetType()
		=> base.GetType();

	/// <summary>
	///     <i>Not supported!</i><br />
	///     <see cref="object.ToString()" /> is not supported.
	/// </summary>
	/// <remarks>
	///     Consider adding support for <see cref="EditorBrowsableAttribute" /> to hide this method from code suggestions.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string? ToString()
		=> base.ToString();

	/// <summary>
	///     Evaluates the expectation as the member of a combination, after the <paramref name="index" /> of the previous
	///     member.
	/// </summary>
	/// <param name="index">The number of the previous member.</param>
	/// <param name="limit">
	///     The time that the combination has left, which tightens the timeout of this evaluation only, or
	///     <see langword="null" /> without a limit.
	/// </param>
	/// <param name="cancellationToken">
	///     The cancellation token of the combination, which this evaluation observes together with its own one, or
	///     <see langword="null" /> when the combination has none.
	/// </param>
	internal abstract ValueTask<Result> GetResult(int index, TimeSpan? limit, CancellationToken? cancellationToken);

	/// <summary>
	///     Ends the evaluation, once the failure message no longer reads the collections it materialized.
	/// </summary>
	internal abstract Task EndEvaluation();

	/// <summary>
	///     Resolves the reasons that must be awaited of the met expectations, before the failure message of a
	///     combination shows them.
	/// </summary>
	internal abstract Task ResolvePendingReasons();

	/// <summary>
	///     Whether a callback was registered with <see cref="IEvaluationContext.OnCompleted" /> during the evaluation of a
	///     member of a combination.
	/// </summary>
	internal abstract bool HasCompletionCallbacks { get; }

	/// <summary>
	///     Runs the callbacks registered with <see cref="IEvaluationContext.OnCompleted" /> during the evaluation of a
	///     member of a combination with the <paramref name="outcome" /> of the combination, skipping and extending the
	///     <paramref name="ran" /> ones.
	/// </summary>
	internal abstract Task RunCompletionCallbacks(Outcome outcome,
		List<Func<CompletedEvaluation, CancellationToken, ValueTask>> ran);

	/// <summary>
	///     Lets the evaluations of the expectation run on the <paramref name="timeProvider" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A time provider is already set.</exception>
	internal abstract void UseTimeProvider(TimeProvider timeProvider);

	/// <summary>
	///     Adds the cancellation of each expectation for what is still awaited after its evaluation, see
	///     <see cref="EvaluationCancellation.ForRemainingTimeout" />, to the <paramref name="cancellations" />. The
	///     caller releases them.
	/// </summary>
	internal abstract void AddRemainingCancellations(List<EvaluationCancellation> cancellations);

	/// <summary>
	///     Adds the formatters of each expectation and of the expectations nested in its current evaluation, see
	///     <see cref="ExpectationBuilder.AddFormatter" />, to the <paramref name="formatters" />.
	/// </summary>
	internal abstract void AddFormatters(List<IValueFormatter> formatters);

	/// <param name="index">The number of the last expectation in the result.</param>
	/// <param name="subject">The subject line, or with <paramref name="isNumbered" /> the subject it names.</param>
	/// <param name="result">The result of the expectation.</param>
	/// <param name="isNumbered">Whether the subject line names the <paramref name="subject" /> with the number.</param>
	internal struct Result(int index, string subject, ConstraintResult result, bool isNumbered = false)
	{
		public int Index { get; } = index;

		/// <remarks>
		///     A numbered subject line is only formatted when a failure message reads it, and names a describable subject
		///     by its description, like the failure message of a single expectation.
		/// </remarks>
		public string SubjectLine => isNumbered ? $" [{Index:00}] Expected that {DescribeSubject()}" : subject;

		public ConstraintResult ConstraintResult { get; } = result;

		private string DescribeSubject()
			=> ConstraintResult.TryGetValue(out IDescribableSubject? describableSubject)
				? describableSubject.GetDescription()
				: subject;
	}

	/// <summary>
	///     Combination of multiple expectations.
	/// </summary>
	public abstract class Combination : Expectation
	{
		private readonly Expectation[] _expectations;

		/// <summary>
		///     The options of the combination, or <see langword="null" /> when none was set.
		/// </summary>
		/// <remarks>
		///     Only allocated when an option is set, so that a combination without options stays small.
		/// </remarks>
		private CombinationOptions? _options;

		/// <summary>
		///     Combination of multiple expectations.
		/// </summary>
		protected Combination(Expectation[] expectations)
		{
			expectations.ThrowIfNull();
			if (expectations.Length == 0)
			{
				throw Tracing.WriteException(
					new ArgumentException("You must provide at least one expectation.", nameof(expectations)));
			}

			for (int i = 0; i < expectations.Length; i++)
			{
				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				if (expectations[i] is null)
				{
					throw Tracing.WriteException(
						new ArgumentException("The 'expectations' cannot contain null.", nameof(expectations)));
				}
			}

			_expectations = expectations;
		}

		/// <summary>
		///     By awaiting the result, the expectations are verified.
		///     <para />
		///     Will throw an exception, when the expectations are not met.
		/// </summary>
		/// <remarks>
		///     Expectations that are met without waiting for anything complete right away, as starting the asynchronous
		///     evaluation would take longer than the expectations themselves.
		/// </remarks>
		public TaskAwaiter GetAwaiter()
		{
			ValueTask<Result> result;
			try
			{
				result = GetResult(0, null, null);
			}
			catch (Exception exception)
			{
				result = new ValueTask<Result>(Task.FromException<Result>(exception));
			}

			if (!result.IsCompletedSuccessfully)
			{
				return GetResultOrThrow(result).GetAwaiter();
			}

			Result completed = result.Result;
			return completed.ConstraintResult.Outcome == Outcome.Success && !HasCompletionCallbacks
				? EndEvaluation().GetAwaiter()
				: GetResultOrThrow(new ValueTask<Result>(completed)).GetAwaiter();
		}

		/// <summary>
		///     Returns the subject line of the <see cref="Expectation.Combination" />.
		/// </summary>
		protected abstract string GetSubjectLine();

		/// <summary>
		///     Adds the <paramref name="reason" /> to the header of the failure message.
		/// </summary>
		private protected void AddReason(IBecauseReason reason)
			=> (GetOptions().Reasons ??= []).Add(reason);

		/// <summary>
		///     Sets the <paramref name="cancellationToken" /> that every member observes.
		/// </summary>
		/// <exception cref="InvalidOperationException">A cancellation token is already set.</exception>
		private protected void SetCancellation(CancellationToken cancellationToken)
		{
			ThrowHelper.ThrowIfAlreadySpecified(_options?.CancellationToken is not null,
				nameof(Combination<All>.WithCancellation));
			GetOptions().CancellationToken = cancellationToken;
		}

		/// <summary>
		///     Limits the whole combination to the <paramref name="timeout" />, unless a tighter one is already set.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
		private protected void LimitTimeout(TimeSpan timeout)
		{
			ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
			CombinationOptions options = GetOptions();
			options.Timeout = TimerHelpers.Tighter(options.Timeout, timeout) ?? timeout;
		}

		private CombinationOptions GetOptions() => _options ??= new CombinationOptions();

		/// <summary>
		///     The subject line with the reasons of the combination, which are read when the failure message is written,
		///     so that a reason that must be awaited is resolved by then.
		/// </summary>
		private string GetHeader()
		{
			string subjectLine = GetSubjectLine();
			if (_options?.Reasons is not { } reasons)
			{
				return subjectLine;
			}

			bool endsWithColon = subjectLine.EndsWith(':');
			StringBuilder header = new(subjectLine, 0,
				endsWithColon ? subjectLine.Length - 1 : subjectLine.Length, subjectLine.Length + 32);
			foreach (IBecauseReason reason in reasons)
			{
				header.Append(reason.ToString());
			}

			return endsWithColon ? header.Append(':').ToString() : header.ToString();
		}

		/// <summary>
		///     Specifies if the combination should be treated as
		///     <see cref="Outcome.Success" />, <see cref="Outcome.Failure" /> or <see cref="Outcome.Undecided" />.
		/// </summary>
		protected abstract Outcome CheckOutcome(Outcome? previous, Outcome current);

		/// <summary>
		///     Combines the <paramref name="own" /> cancellation token with the one of an outer
		///     <paramref name="combination" />, so that both cancel the evaluation.
		/// </summary>
		/// <remarks>
		///     Only when both can be canceled, a <paramref name="linked" /> source is created, which the caller releases at
		///     the end of the evaluation.
		/// </remarks>
		internal static CancellationToken? CombineCancellation(CancellationToken? own, CancellationToken? combination,
			out CancellationTokenSource? linked)
		{
			linked = null;
			if (own is not { } ownToken)
			{
				return combination;
			}

			if (combination is not { CanBeCanceled: true, } combinationToken || combinationToken == ownToken)
			{
				return ownToken;
			}

			if (!ownToken.CanBeCanceled)
			{
				return combinationToken;
			}

			linked = CancellationTokenSource.CreateLinkedTokenSource(ownToken, combinationToken);
			return linked.Token;
		}

		/// <inheritdoc />
		/// <remarks>
		///     The members are only awaited from the first one that does not complete right away.
		///     <para />
		///     The members are evaluated one after another, so the timeout of the combination is one deadline for all of
		///     them: each member only gets the time that is left.
		/// </remarks>
		internal override ValueTask<Result> GetResult(int index, TimeSpan? limit, CancellationToken? cancellationToken)
		{
			MemberLimits limits = StartEvaluation(limit, cancellationToken);
			(Expectation Expectation, Result Result)[] results = new (Expectation, Result)[_expectations.Length];
			Exception? failureCause = null;
			Outcome? outcome = null;
			for (int i = 0; i < _expectations.Length; i++)
			{
				ValueTask<Result> result =
					_expectations[i].GetResult(index, limits.GetRemainingTime(), limits.CancellationToken);
				if (!result.IsCompletedSuccessfully)
				{
					return GetResultAsync(result, i, results, failureCause, outcome, limits);
				}

				index = AddResult(i, result.Result, results, ref failureCause, ref outcome);
			}

			return new ValueTask<Result>(CreateResult(index, results, failureCause, outcome));
		}

		private async ValueTask<Result> GetResultAsync(ValueTask<Result> pending, int position,
			(Expectation Expectation, Result Result)[] results, Exception? failureCause, Outcome? outcome,
			MemberLimits limits)
		{
			Result result = await pending;
			int index = AddResult(position, result, results, ref failureCause, ref outcome);
			for (int i = position + 1; i < _expectations.Length; i++)
			{
				result = await _expectations[i].GetResult(index, limits.GetRemainingTime(), limits.CancellationToken);
				index = AddResult(i, result, results, ref failureCause, ref outcome);
			}

			return CreateResult(index, results, failureCause, outcome);
		}

		/// <summary>
		///     Starts the deadline of the combination, from the tighter of its own timeout and the
		///     <paramref name="limit" /> of an outer combination, and combines the cancellation tokens.
		/// </summary>
		private MemberLimits StartEvaluation(TimeSpan? limit, CancellationToken? cancellationToken)
		{
			CombinationOptions? options = _options;
			if (options is null)
			{
				return limit is null && cancellationToken is null
					? default
					: new MemberLimits(limit, TimeProvider.System, cancellationToken);
			}

			options.ReleaseLinkedCancellation();
			CancellationToken? token = CombineCancellation(options.CancellationToken, cancellationToken,
				out CancellationTokenSource? linked);
			options.LinkedCancellation = linked;
			TimeSpan? timeout = TimerHelpers.Tighter(limit, options.Timeout);
			return new MemberLimits(timeout == System.Threading.Timeout.InfiniteTimeSpan ? null : timeout,
				options.TimeProvider ?? TimeProvider.System, token);
		}

		private int AddResult(int position, Result result, (Expectation Expectation, Result Result)[] results,
			ref Exception? failureCause, ref Outcome? outcome)
		{
			// The combination is never negated, so an expectation that fails both ways only fails it.
			Outcome current = result.ConstraintResult.Outcome == Outcome.FailureBothWays
				? Outcome.Failure
				: result.ConstraintResult.Outcome;
			outcome = CheckOutcome(outcome, current);
			if (current == Outcome.Failure)
			{
				failureCause ??= result.ConstraintResult.FailureCause;
			}

			results[position] = (_expectations[position], result);
			return result.Index;
		}

		private Result CreateResult(int index, (Expectation Expectation, Result Result)[] results,
			Exception? failureCause, Outcome? outcome)
			=> new(index, GetSubjectLine(), outcome is Outcome.Failure or Outcome.Undecided
				? new CombinationResult(outcome.Value, results, failureCause)
				: new CombinationResult(Outcome.Success, results));

		/// <inheritdoc />
		internal override Task EndEvaluation()
		{
			_options?.ReleaseLinkedCancellation();
			for (int i = 0; i < _expectations.Length; i++)
			{
				Task endEvaluation = _expectations[i].EndEvaluation();
				if (endEvaluation.Status != TaskStatus.RanToCompletion)
				{
					return EndEvaluationAsync(endEvaluation, i);
				}
			}

			return Task.CompletedTask;
		}

		private async Task EndEvaluationAsync(Task pending, int position)
		{
			await pending;
			for (int i = position + 1; i < _expectations.Length; i++)
			{
				await _expectations[i].EndEvaluation();
			}
		}

		/// <inheritdoc />
		/// <remarks>
		///     The own reasons of the combination are awaited until the first timeout of the expectations elapses or one
		///     of them is canceled, like its contexts.
		/// </remarks>
		internal override async Task ResolvePendingReasons()
		{
			foreach (Expectation expectation in _expectations)
			{
				await expectation.ResolvePendingReasons();
			}

			if (_options?.Reasons is { } reasons && reasons.Exists(reason => reason is AsyncBecauseReason))
			{
				await WithRemainingCancellation(async token =>
				{
					foreach (IBecauseReason reason in reasons)
					{
						if (reason is AsyncBecauseReason asyncReason)
						{
							await asyncReason.Resolve(token);
						}
					}
				});
			}
		}

		/// <inheritdoc />
		internal override bool HasCompletionCallbacks
		{
			get
			{
				foreach (Expectation expectation in _expectations)
				{
					if (expectation.HasCompletionCallbacks)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <inheritdoc />
		/// <remarks>
		///     Each combined expectation passes the cancellation token of its own caller to its callbacks.
		/// </remarks>
		internal override async Task RunCompletionCallbacks(Outcome outcome,
			List<Func<CompletedEvaluation, CancellationToken, ValueTask>> ran)
		{
			foreach (Expectation expectation in _expectations)
			{
				await expectation.RunCompletionCallbacks(outcome, ran);
			}
		}

		/// <inheritdoc />
		/// <remarks>
		///     A combination has no evaluation of its own, so every combined expectation runs on the
		///     <paramref name="timeProvider" />.
		/// </remarks>
		internal override void UseTimeProvider(TimeProvider timeProvider)
		{
			foreach (Expectation expectation in _expectations)
			{
				expectation.UseTimeProvider(timeProvider);
			}

			GetOptions().TimeProvider = timeProvider;
		}

		/// <inheritdoc />
		internal override void AddRemainingCancellations(List<EvaluationCancellation> cancellations)
		{
			foreach (Expectation expectation in _expectations)
			{
				expectation.AddRemainingCancellations(cancellations);
			}
		}

		/// <inheritdoc />
		internal override void AddFormatters(List<IValueFormatter> formatters)
		{
			foreach (Expectation expectation in _expectations)
			{
				expectation.AddFormatters(formatters);
			}
		}

		private async Task GetResultOrThrow(ValueTask<Result> isMet)
		{
			try
			{
				await ThrowUnlessMet(isMet);
			}
			finally
			{
				await EndEvaluation();
			}
		}

		private async Task ThrowUnlessMet(ValueTask<Result> isMet)
		{
			Result result = await isMet;
			if (HasCompletionCallbacks)
			{
				await RunCompletionCallbacks(result.ConstraintResult.Outcome, []);
			}

			if (result.ConstraintResult.Outcome == Outcome.Success)
			{
				return;
			}

			await ResolvePendingReasons();
			List<IValueFormatter> formatters = [];
			AddFormatters(formatters);
			using ValueFormatter.FormatterScope _ =
				ValueFormatter.UseScopedFormatters(formatters.Count > 0 ? formatters.ToArray() : null);
			StringBuilder sb = new();
			sb.AppendLine(GetHeader());
			result.ConstraintResult.AppendExpectation(sb);
			sb.AppendLine();
			sb.AppendLine("but");
			result.ConstraintResult.AppendResult(sb);
			await AppendContexts(sb, result.ConstraintResult);

			if (result.ConstraintResult.Outcome == Outcome.Undecided)
			{
				Fail.Inconclusive(sb.ToString());
			}

			Fail.Test(sb.ToString(), result.ConstraintResult.FailureCause);
		}

		/// <remarks>
		///     A context is awaited until the first timeout of the expectations elapses or one of them is canceled, as
		///     the contexts of all expectations are rendered together.
		/// </remarks>
		private Task AppendContexts(StringBuilder stringBuilder, ConstraintResult failure)
			=> WithRemainingCancellation(token => ResultContextRenderer.AppendContexts(stringBuilder, failure, token));

		/// <summary>
		///     Runs the <paramref name="action" /> with a token that is canceled when the first timeout of the
		///     expectations elapses or one of them is canceled.
		/// </summary>
		/// <remarks>
		///     The members only get the time that is left of the timeout of the combination and observe its cancellation
		///     token, so their remaining cancellations also cover those of the combination.
		/// </remarks>
		private async Task WithRemainingCancellation(Func<CancellationToken, Task> action)
		{
			List<EvaluationCancellation> cancellations = [];
			AddRemainingCancellations(cancellations);
			CancellationToken[] tokens = cancellations.Select(remaining => remaining.Token)
				.Where(token => token.CanBeCanceled).ToArray();
			using CancellationTokenSource? cancellation =
				tokens.Length > 0 ? CancellationTokenSource.CreateLinkedTokenSource(tokens) : null;
			try
			{
				await action(cancellation?.Token ?? CancellationToken.None);
			}
			finally
			{
				foreach (EvaluationCancellation remaining in cancellations)
				{
					remaining.Release();
				}
			}
		}

		/// <remarks>
		///     The texts of the members are only rendered when they are read, as a combination that succeeds never shows
		///     them.
		/// </remarks>
		private sealed class CombinationResult : ConstraintResult
		{
			private readonly (Expectation Expectation, Result Result)[] _results;

			public CombinationResult(Outcome outcome, (Expectation Expectation, Result Result)[] results,
				Exception? failureCause = null)
				: base(ExpectationGrammars.None)
			{
				_results = results;
				FailureCause = failureCause;
				Outcome = outcome;
			}

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause { get; }

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(GetExpectationTexts().Indent(indentation, false));

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (Outcome != Outcome.Success)
				{
					stringBuilder.Append(GetFailureTexts().Indent(indentation, false));
				}
			}

			private string GetExpectationTexts()
			{
				StringBuilder expectationTexts = new();
				foreach ((Expectation expectation, Result result) in _results)
				{
					if (expectationTexts.Length > 0)
					{
						expectationTexts.AppendLine();
					}

					if (expectation is Combination combination)
					{
						expectationTexts.Append("  ").Append(combination.GetHeader()).AppendLine().Append("  ");
						result.ConstraintResult.AppendExpectation(expectationTexts, "  ");
					}
					else
					{
						expectationTexts.Append(result.SubjectLine).Append(' ');
						result.ConstraintResult.AppendExpectation(expectationTexts, "      ");
					}
				}

				return expectationTexts.ToString();
			}

			private string GetFailureTexts()
			{
				StringBuilder failureTexts = new();
				foreach ((Expectation expectation, Result result) in _results)
				{
					if (result.ConstraintResult.Outcome != Outcome.Success)
					{
						AppendFailureText(failureTexts, expectation, result);
					}
				}

				return failureTexts.ToString();
			}

			private static void AppendFailureText(StringBuilder failureTexts, Expectation expectation, Result result)
			{
				if (failureTexts.Length > 0)
				{
					failureTexts.AppendLine();
				}

				if (expectation is Combination)
				{
					failureTexts.Append("  ");
					result.ConstraintResult.AppendResult(failureTexts, "  ");
				}
				else
				{
					failureTexts.Append(" [").Append(result.Index.ToString("00")).Append("] ");
					result.ConstraintResult.AppendResult(failureTexts, "      ");
				}
			}

			public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			{
				value = default;
				return false;
			}

			protected internal override void NegateInPlace()
			{
				// A combination of expectations is never negated.
			}

			/// <remarks>
			///     The contexts of each failed expectation are numbered like it, while a nested combination numbers its own.
			/// </remarks>
			public override void AppendContexts(ResultContextCollector contexts)
			{
				foreach ((Expectation expectation, Result result) in _results)
				{
					if (result.ConstraintResult.Outcome == Outcome.Success)
					{
						continue;
					}

					if (expectation is Combination)
					{
						contexts.Visit(result.ConstraintResult);
					}
					else
					{
						contexts.VisitWithTitlePrefix($"[{result.Index:00}] ", result.ConstraintResult);
					}
				}
			}
		}

		/// <summary>
		///     The options of a combination that only few combinations use.
		/// </summary>
		private sealed class CombinationOptions
		{
			public CancellationToken? CancellationToken { get; set; }

			/// <summary>
			///     The source that links the <see cref="CancellationToken" /> with the one of an outer combination during
			///     the current evaluation.
			/// </summary>
			public CancellationTokenSource? LinkedCancellation { get; set; }

			public List<IBecauseReason>? Reasons { get; set; }
			public TimeSpan? Timeout { get; set; }
			public TimeProvider? TimeProvider { get; set; }

			public void ReleaseLinkedCancellation()
			{
				LinkedCancellation?.Dispose();
				LinkedCancellation = null;
			}
		}

		/// <summary>
		///     The deadline and the cancellation token of one evaluation of a combination, which its members observe.
		/// </summary>
		private readonly struct MemberLimits
		{
			private readonly long _start;
			private readonly TimeSpan? _timeout;
			private readonly TimeProvider? _timeProvider;

			public MemberLimits(TimeSpan? timeout, TimeProvider timeProvider, CancellationToken? cancellationToken)
			{
				_timeout = timeout;
				CancellationToken = cancellationToken;
				if (timeout is not null)
				{
					_timeProvider = timeProvider;
					_start = timeProvider.GetTimestamp();
				}
			}

			public CancellationToken? CancellationToken { get; }

			/// <summary>
			///     The time until the deadline, which is zero once it passed, or <see langword="null" /> without one.
			/// </summary>
			public TimeSpan? GetRemainingTime()
			{
				if (_timeout is not { } timeout)
				{
					return null;
				}

				TimeSpan remaining = timeout - _timeProvider!.GetElapsedTime(_start);
				return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
			}
		}

		/// <summary>
		///     All <paramref name="expectations" /> must be met.
		/// </summary>
		public class All(Expectation[] expectations) : Combination<All>(expectations)
		{
			/// <inheritdoc />
			protected override string GetSubjectLine()
				=> "Expected all of the following to succeed:";

			/// <inheritdoc />
			protected override Outcome CheckOutcome(Outcome? previous, Outcome current)
				=> (previous, current) switch
				{
					(Outcome.Failure, _) => Outcome.Failure,
					(_, Outcome.Failure) => Outcome.Failure,
					(Outcome.Undecided, _) => Outcome.Undecided,
					(_, Outcome.Undecided) => Outcome.Undecided,
					(_, _) => Outcome.Success,
				};
		}

		/// <summary>
		///     Any of the <paramref name="expectations" /> must be met.
		/// </summary>
		public class Any(Expectation[] expectations) : Combination<Any>(expectations)
		{
			/// <inheritdoc />
			protected override string GetSubjectLine()
				=> "Expected any of the following to succeed:";

			/// <inheritdoc />
			protected override Outcome CheckOutcome(Outcome? previous, Outcome current)
				=> (previous, current) switch
				{
					(Outcome.Success, _) => Outcome.Success,
					(_, Outcome.Success) => Outcome.Success,
					(Outcome.Undecided, _) => Outcome.Undecided,
					(_, Outcome.Undecided) => Outcome.Undecided,
					(_, _) => Outcome.Failure,
				};
		}
	}

	/// <summary>
	///     Combination of multiple expectations, whose options return the <typeparamref name="TSelf" />, so that they can
	///     be chained.
	/// </summary>
	public abstract class Combination<TSelf> : Combination
		where TSelf : Combination<TSelf>
	{
		/// <summary>
		///     Combination of multiple expectations.
		/// </summary>
		protected Combination(Expectation[] expectations) : base(expectations)
		{
		}

		/// <summary>
		///     Provide a <paramref name="reason" /> explaining why the combination is needed.<br />
		///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </summary>
		/// <remarks>
		///     When the <paramref name="reason" /> is <see langword="null" />, empty or only whitespace, it is ignored.
		///     <para />
		///     The reason is shown in the first line of the failure message, e.g.
		///     <c>Expected all of the following to succeed, because …:</c>, and not on the combined expectations, which
		///     keep their own reasons.
		/// </remarks>
		public TSelf Because(string? reason)
		{
			if (!string.IsNullOrWhiteSpace(reason))
			{
				AddReason(new BecauseReason(reason!));
			}

			return (TSelf)this;
		}

		/// <summary>
		///     Provide an <see langword="async" /> <paramref name="reason" /> explaining why the combination is needed.<br />
		///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </summary>
		/// <remarks>
		///     When the <paramref name="reason" /> resolves to <see langword="null" />, empty or only whitespace, it is ignored.
		///     A <paramref name="reason" /> that is still pending when the timeout elapses or the evaluation is canceled is
		///     not awaited any longer, and the failure message states that the reason was not available in time.
		///     <para />
		///     The reason is shown in the first line of the failure message, e.g.
		///     <c>Expected all of the following to succeed, because …:</c>, and not on the combined expectations, which
		///     keep their own reasons. It is only awaited when the combination fails.
		/// </remarks>
		/// <exception cref="ArgumentNullException">The <paramref name="reason" /> is <see langword="null" />.</exception>
		public TSelf Because(Task<string?> reason)
		{
			reason.ThrowIfNull();
			AsyncBecauseReason asyncReason = new(reason);
			asyncReason.ObserveExceptions();
			AddReason(asyncReason);
			return (TSelf)this;
		}

		/// <summary>
		///     Sets the <paramref name="cancellationToken" /> to be passed to every combined expectation.
		/// </summary>
		/// <remarks>
		///     An awaited task, such as a <see cref="Task{TResult}" /> subject or the task of an asynchronous delegate, is
		///     abandoned when the <paramref name="cancellationToken" /> is canceled before it completes. An expectation that
		///     is interrupted by the cancellation is inconclusive: it could not be verified, because the evaluation was
		///     already canceled.
		///     <para />
		///     A combined expectation with a cancellation token of its own observes both tokens. The
		///     <paramref name="cancellationToken" /> replaces the token of
		///     <c>Customize.aweXpect.Global.Settings().TestCancellation</c> for the combined expectations, like the one of
		///     a single expectation.
		/// </remarks>
		/// <exception cref="InvalidOperationException">A cancellation token is already set.</exception>
		public TSelf WithCancellation(CancellationToken cancellationToken)
		{
			SetCancellation(cancellationToken);
			return (TSelf)this;
		}

		/// <summary>
		///     Sets the <paramref name="timeout" /> for the whole combination.
		/// </summary>
		/// <remarks>
		///     The combined expectations are evaluated one after another, so the <paramref name="timeout" /> is one
		///     deadline for all of them, measured from the start of the evaluation: each expectation only gets the time
		///     that is left. Once it elapsed, an expectation that is met or fails without waiting is still verified, while
		///     one that would wait fails with <c>did not finish within …</c>, like with <c>WithTimeout(TimeSpan.Zero)</c>.
		///     <para />
		///     The tightest limit wins: a longer <paramref name="timeout" /> does not loosen an earlier one, the timeout of
		///     a combined expectation, the timeout of an outer combination or the global timeout.
		///     <see cref="Timeout.InfiniteTimeSpan" /> imposes no limit.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
		public TSelf WithTimeout(TimeSpan timeout)
		{
			LimitTimeout(timeout);
			return (TSelf)this;
		}
	}
}
