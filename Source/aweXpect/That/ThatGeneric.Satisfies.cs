using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the actual value to satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> decides about a <see langword="null" /> subject as well, so
	///     <c>Satisfies(x =&gt; x is null)</c> succeeds. A <paramref name="predicate" /> that throws fails the
	///     expectation with the thrown exception as inner exception.
	/// </remarks>
	public static RepeatedCheckResult<T, IThat<T>> Satisfies<T>(this IThat<T> subject,
		Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new SatisfiesConstraint<T>(
						it,
						grammars,
						predicate,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies the actual value to not satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> decides about a <see langword="null" /> subject as well, so
	///     <c>DoesNotSatisfy(x =&gt; x is not null)</c> succeeds. A <paramref name="predicate" /> that throws fails the
	///     expectation with the thrown exception as inner exception.
	/// </remarks>
	public static RepeatedCheckResult<T, IThat<T>> DoesNotSatisfy<T>(this IThat<T> subject,
		Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new SatisfiesConstraint<T>(
						it,
						grammars,
						predicate,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						options).Invert()),
			subject,
			options);
	}

	/// <remarks>
	///     It derives from <see cref="ConstraintResult.WithValue{T}" /> and not from
	///     <see cref="ConstraintResult.WithNotNullValue{T}" />, because the predicate is the caller's own inspection of
	///     the subject and states itself how a <see langword="null" /> is to be treated.
	/// </remarks>
	private sealed class SatisfiesConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		Func<T, bool> predicate,
		string predicateExpression,
		RepeatedCheckOptions options)
		: ConstraintResult.WithValue<T>(it, grammars),
			IAsyncConstraint<T>
	{
		private Exception? _exception;

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		/// <remarks>
		///     A predicate that threw answered nothing, so it fails the expectation and its negation alike.
		/// </remarks>
		public override Outcome Outcome
		{
			get => _exception is null ? base.Outcome : Outcome.Failure;
			protected set => base.Outcome = value;
		}

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => _exception;

		public async Task<ConstraintResult> IsMetBy(T actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (IsMet(actual, cancellationToken) || _exception is not null)
			{
				return this;
			}

			if (options.Timeout > TimeSpan.Zero)
			{
				Stopwatch sw = new();
				sw.Start();
				do
				{
					try
					{
						await Task.Delay(options.Interval.NextCheckInterval(), cancellationToken);
					}
					catch (TaskCanceledException)
					{
						break;
					}

					if (IsMet(actual, cancellationToken) || _exception is not null)
					{
						return this;
					}
				} while (sw.Elapsed <= options.Timeout && !cancellationToken.IsCancellationRequested);
			}

			return this;
		}

		private bool IsMet(T actual, CancellationToken cancellationToken)
		{
			bool isSatisfied;
			try
			{
				isSatisfied = predicate(actual);
			}
			catch (Exception exception) when (exception is not OperationCanceledException ||
			                                  !cancellationToken.IsCancellationRequested)
			{
				_exception = exception;
				return false;
			}

			// The base class negates the outcome on read, so the raw predicate result is stored here.
			Outcome = isSatisfied ? Outcome.Success : Outcome.Failure;
			return isSatisfied != IsNegated;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("satisfies ", "satisfy ")).Append(predicateExpression.TrimCommonWhiteSpace())
				.Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_exception is not null)
			{
				stringBuilder.Append(It).Append(" did throw ").Append(_exception.FormatForMessage(indentation));
				return;
			}

			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not satisfy ", "do not satisfy ")).Append(predicateExpression.TrimCommonWhiteSpace())
				.Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
