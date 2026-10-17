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
using aweXpect.Core.Internal;

namespace aweXpect.Core;

/// <summary>
///     Base class for expectation results.
/// </summary>
/// <remarks>
///     Create instances by using the static methods on the <see cref="Expect" /> class.
/// </remarks>
[StackTraceHidden]
public abstract class Expectation
{
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

	internal abstract ValueTask<Result> GetResult(int index);

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
	///     Lets the evaluations of the expectation run on the <paramref name="timeSystem" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A time system is already set.</exception>
	internal abstract void UseTimeSystem(ITimeSystem timeSystem);

	/// <summary>
	///     Adds the cancellation of each expectation for what is still awaited after its evaluation, see
	///     <see cref="EvaluationCancellation.ForRemainingTimeout" />, to the <paramref name="cancellations" />. The
	///     caller releases them.
	/// </summary>
	internal abstract void AddRemainingCancellations(List<EvaluationCancellation> cancellations);

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
				result = GetResult(0);
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
		///     Specifies if the combination should be treated as
		///     <see cref="Outcome.Success" />, <see cref="Outcome.Failure" /> or <see cref="Outcome.Undecided" />.
		/// </summary>
		protected abstract Outcome CheckOutcome(Outcome? previous, Outcome current);

		/// <inheritdoc />
		/// <remarks>
		///     The members are only awaited from the first one that does not complete right away.
		/// </remarks>
		internal override ValueTask<Result> GetResult(int index)
		{
			(Expectation Expectation, Result Result)[] results = new (Expectation, Result)[_expectations.Length];
			Exception? failureCause = null;
			Outcome? outcome = null;
			for (int i = 0; i < _expectations.Length; i++)
			{
				ValueTask<Result> result = _expectations[i].GetResult(index);
				if (!result.IsCompletedSuccessfully)
				{
					return GetResultAsync(result, i, results, failureCause, outcome);
				}

				index = AddResult(i, result.Result, results, ref failureCause, ref outcome);
			}

			return new ValueTask<Result>(CreateResult(index, results, failureCause, outcome));
		}

		private async ValueTask<Result> GetResultAsync(ValueTask<Result> pending, int position,
			(Expectation Expectation, Result Result)[] results, Exception? failureCause, Outcome? outcome)
		{
			Result result = await pending;
			int index = AddResult(position, result, results, ref failureCause, ref outcome);
			for (int i = position + 1; i < _expectations.Length; i++)
			{
				result = await _expectations[i].GetResult(index);
				index = AddResult(i, result, results, ref failureCause, ref outcome);
			}

			return CreateResult(index, results, failureCause, outcome);
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
		internal override async Task ResolvePendingReasons()
		{
			foreach (Expectation expectation in _expectations)
			{
				await expectation.ResolvePendingReasons();
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
		///     <paramref name="timeSystem" />.
		/// </remarks>
		internal override void UseTimeSystem(ITimeSystem timeSystem)
		{
			foreach (Expectation expectation in _expectations)
			{
				expectation.UseTimeSystem(timeSystem);
			}
		}

		/// <inheritdoc />
		internal override void AddRemainingCancellations(List<EvaluationCancellation> cancellations)
		{
			foreach (Expectation expectation in _expectations)
			{
				expectation.AddRemainingCancellations(cancellations);
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
			StringBuilder sb = new();
			sb.AppendLine(GetSubjectLine());
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
		private async Task AppendContexts(StringBuilder stringBuilder, ConstraintResult failure)
		{
			List<EvaluationCancellation> cancellations = [];
			AddRemainingCancellations(cancellations);
			CancellationToken[] tokens = cancellations.Select(remaining => remaining.Token)
				.Where(token => token.CanBeCanceled).ToArray();
			using CancellationTokenSource? cancellation =
				tokens.Length > 0 ? CancellationTokenSource.CreateLinkedTokenSource(tokens) : null;
			try
			{
				await ResultContextRenderer.AppendContexts(stringBuilder, failure,
					cancellation?.Token ?? CancellationToken.None);
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

					if (expectation is Combination)
					{
						expectationTexts.Append("  ").Append(result.SubjectLine).AppendLine().Append("  ");
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

			public override ConstraintResult Negate() => this;

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
		///     All <paramref name="expectations" /> must be met.
		/// </summary>
		public class All(Expectation[] expectations) : Combination(expectations)
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
		public class Any(Expectation[] expectations) : Combination(expectations)
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
}
