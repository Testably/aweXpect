using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Results;

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
		=> throw new NotSupportedException("Equals is not supported. Did you mean Is() instead?");
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
		=> throw new NotSupportedException("GetHashCode is not supported.");
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

	internal struct Result(int index, string subjectLine, ConstraintResult result)
	{
		public int Index { get; } = index;
		public string SubjectLine { get; } = subjectLine;
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
			if (expectations.Length == 0)
			{
				throw new ArgumentException("You must provide at least one expectation.", nameof(expectations))
					.LogTrace();
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
		///     Specifies, if the combination should be treated as
		///     <see cref="Outcome.Success" /> or <see cref="Outcome.Failure" />
		/// </summary>
		protected abstract Outcome CheckOutcome(Outcome? previous, Outcome current);

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
		/// <inheritdoc />
		internal override async Task<Result> GetResult(int index, Dictionary<int, Outcome> outcomes)
		{
			StringBuilder expectationTexts = new();
			StringBuilder failureTexts = new();
			Exception? failureCause = null;
			Outcome? outcome = null;
			foreach (Expectation? expectation in _expectations)
			{
				Result result = await expectation.GetResult(index, outcomes);
				outcome = CheckOutcome(outcome, result.ConstraintResult.Outcome);
				index = result.Index;
				outcomes[index] = result.ConstraintResult.Outcome;
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

				if (result.ConstraintResult.Outcome == Outcome.Failure)
				{
					failureCause ??= result.ConstraintResult.FailureCause;
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
						failureTexts.Append(" [").Append(index.ToString("00")).Append("] ");
						result.ConstraintResult.AppendResult(failureTexts, "      ");
					}
				}
			}

			if (outcome != Outcome.Success)
			{
				return new Result(index, GetSubjectLine(),
					new CombinationResult(Outcome.Failure, expectationTexts.ToString(), failureTexts.ToString(),
						failureCause));
			}

			return new Result(index, GetSubjectLine(),
				new CombinationResult(Outcome.Success, expectationTexts.ToString()));
		}
#pragma warning restore S3776

		internal override IEnumerable<ResultContext> GetContexts(int index, Dictionary<int, Outcome> outcomes)
		{
			List<ResultContext> combinedContexts = new();
			foreach (Expectation expectation in _expectations)
			{
				if (expectation is Combination)
				{
					combinedContexts.AddRange(expectation.GetContexts(index, outcomes));
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

			return combinedContexts;
		}

		private async Task GetResultOrThrow(CancellationToken cancellationToken = default)
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
				string? content = await context.GetContent(cancellationToken);
				if (content is null)
				{
					continue;
				}

				sb.AppendLine().AppendLine();
				sb.Append(context.Title).Append(':').AppendLine();
				sb.Append(content);
			}

			Fail.Test(sb.ToString(), result.ConstraintResult.FailureCause);
		}

		private sealed class CombinationResult : ConstraintResult
		{
			private readonly string _expectationTexts;
			private readonly string? _failureTexts;

			public CombinationResult(Outcome outcome, string expectationTexts, string? failureTexts = null,
				Exception? failureCause = null)
				: base(ExpectationGrammars.None)
			{
				_expectationTexts = expectationTexts;
				_failureTexts = failureTexts;
				FailureCause = failureCause;
				Outcome = outcome;
			}

			/// <inheritdoc cref="ConstraintResult.FailureCause" />
			public override Exception? FailureCause { get; }

			public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
				=> stringBuilder.Append(_expectationTexts.Indent(indentation, false));

			public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			{
				if (_failureTexts != null)
				{
					stringBuilder.Append(_failureTexts.Indent(indentation, false));
				}
			}

			public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
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
				=> previous switch
				{
					Outcome.Failure => Outcome.Failure,
					_ => current,
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
				=> previous switch
				{
					Outcome.Success => Outcome.Success,
					_ => current,
				};
		}
	}
}
