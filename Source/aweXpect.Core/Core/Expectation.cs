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
using aweXpect.Core.Helpers;

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

	internal abstract Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes);

	internal abstract IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes);

	/// <summary>
	///     Ends the evaluation, once the failure message no longer reads the collections it materialized.
	/// </summary>
	internal abstract Task EndEvaluation();

	/// <param name="index">The number of the last expectation in the result.</param>
	/// <param name="subject">The subject line, or with <paramref name="isNumbered" /> the subject it names.</param>
	/// <param name="result">The result of the expectation.</param>
	/// <param name="isNumbered">Whether the subject line names the <paramref name="subject" /> with the number.</param>
	internal struct Result(int index, string subject, ConstraintResult result, bool isNumbered = false)
	{
		public int Index { get; } = index;

		/// <remarks>
		///     A numbered subject line is only formatted when a failure message reads it.
		/// </remarks>
		public string SubjectLine => isNumbered ? $" [{Index:00}] Expected that {subject}" : subject;

		public ConstraintResult ConstraintResult { get; } = result;
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

			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (expectations.Any(expectation => expectation is null))
			{
				throw Tracing.WriteException(
					new ArgumentException("The 'expectations' cannot contain null.", nameof(expectations)));
			}

			_expectations = expectations;
		}

		/// <summary>
		///     By awaiting the result, the expectations are verified.
		///     <para />
		///     Will throw an exception, when the expectations are not met.
		/// </summary>
		public TaskAwaiter GetAwaiter()
		{
			Task result = GetResultOrThrow();
			return result.GetAwaiter();
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
		internal override async Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes)
		{
			(Expectation Expectation, Result Result)[] results = new (Expectation, Result)[_expectations.Length];
			Exception? failureCause = null;
			Outcome? outcome = null;
			for (int i = 0; i < _expectations.Length; i++)
			{
				Expectation expectation = _expectations[i];
				int firstIndex = index + 1;
				Result result = await expectation.GetResult(index, outcomes);
				outcome = CheckOutcome(outcome, result.ConstraintResult.Outcome);
				index = result.Index;
				RecordOutcome(expectation, firstIndex, result, outcomes);
				if (result.ConstraintResult.Outcome == Outcome.Failure)
				{
					failureCause ??= result.ConstraintResult.FailureCause;
				}

				results[i] = (expectation, result);
			}

			return new Result(index, GetSubjectLine(), outcome is Outcome.Failure or Outcome.Undecided
				? new CombinationResult(outcome.Value, results, failureCause)
				: new CombinationResult(Outcome.Success, results));
		}

		private static string GetExpectationTexts((Expectation Expectation, Result Result)[] results)
		{
			StringBuilder expectationTexts = new();
			foreach ((Expectation expectation, Result result) in results)
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

		private static string GetFailureTexts((Expectation Expectation, Result Result)[] results)
		{
			StringBuilder failureTexts = new();
			foreach ((Expectation expectation, Result result) in results)
			{
				if (result.ConstraintResult.Outcome != Outcome.Success)
				{
					AppendFailureText(failureTexts, expectation, result);
				}
			}

			return failureTexts.ToString();
		}

		internal override IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes)
		{
			List<ResultContext> combinedContexts = new();
			AddContexts(index, outcomes, combinedContexts);
			return combinedContexts;
		}

		/// <summary>
		///     Adds the contexts of the expectations that did not succeed and returns the index of the last expectation,
		///     so that nested combinations are numbered like in <see cref="GetResult" />.
		/// </summary>
		private int AddContexts(int index, Dictionary<int, Outcome> outcomes, List<ResultContext> combinedContexts)
		{
			foreach (Expectation expectation in _expectations)
			{
				if (expectation is Combination combination)
				{
					index = combination.AddContexts(index, outcomes, combinedContexts);
				}
				else
				{
					index++;
					if (outcomes.TryGetValue(index, out Outcome outcome) && outcome == Outcome.Success)
					{
						continue;
					}

					foreach (ResultContext context in expectation.GetContexts(index, outcomes))
					{
						context.Title = $"[{index:00}] {context.Title}";
						combinedContexts.Add(context);
					}
				}
			}

			return index;
		}

		/// <inheritdoc />
		internal override async Task EndEvaluation()
		{
			foreach (Expectation expectation in _expectations)
			{
				await expectation.EndEvaluation();
			}
		}

		private static void RecordOutcome(Expectation expectation, int firstIndex, Result result,
			Dictionary<int, Outcome> outcomes)
		{
			if (expectation is not Combination)
			{
				outcomes[result.Index] = result.ConstraintResult.Outcome;
				return;
			}

			// The failures of the members of a succeeded combination are not reported, so neither are their contexts.
			if (result.ConstraintResult.Outcome == Outcome.Success)
			{
				for (int index = firstIndex; index <= result.Index; index++)
				{
					outcomes[index] = Outcome.Success;
				}
			}
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

		private async Task GetResultOrThrow(CancellationToken cancellationToken = default)
		{
			try
			{
				await ThrowUnlessMet(cancellationToken);
			}
			finally
			{
				await EndEvaluation();
			}
		}

		private async Task ThrowUnlessMet(CancellationToken cancellationToken)
		{
			Dictionary<int, Outcome> outcomes = new();
			Result result = await GetResult(0, outcomes);
			if (result.ConstraintResult.Outcome == Outcome.Success)
			{
				return;
			}

			StringBuilder sb = new();
			sb.AppendLine(GetSubjectLine());
			result.ConstraintResult.AppendExpectation(sb);
			sb.AppendLine();
			sb.AppendLine("but");
			result.ConstraintResult.AppendResult(sb);
			foreach (ResultContext context in GetContexts(0, outcomes).OrderByDescending(x => x.Priority))
			{
				string? content = await context.GetContentUnlessUserCodeThrows(cancellationToken);
				if (content is null)
				{
					continue;
				}

				sb.AppendLine().AppendLine();
				sb.Append(context.Title).Append(':').AppendLine();
				sb.Append(content);
			}

			if (result.ConstraintResult.Outcome == Outcome.Undecided)
			{
				Fail.Inconclusive(sb.ToString());
			}

			Fail.Test(sb.ToString(), result.ConstraintResult.FailureCause);
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
				=> stringBuilder.Append(GetExpectationTexts(_results).Indent(indentation, false));

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (Outcome != Outcome.Success)
				{
					stringBuilder.Append(GetFailureTexts(_results).Indent(indentation, false));
				}
			}

			public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			{
				value = default;
				return false;
			}

			public override ConstraintResult Negate() => this;
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
