using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies that the subject satisfies the <paramref name="predicate" />.
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
				.AddConstraint((Predicate: predicate, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
					static (state, it, grammars) =>
						new SatisfiesConstraint<T>(
							it,
							grammars,
							state.Predicate,
							state.DoNotPopulateThisValue.TrimCommonWhiteSpace(),
							state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not satisfy the <paramref name="predicate" />.
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
				.AddConstraint((Predicate: predicate, DoNotPopulateThisValue: doNotPopulateThisValue, Options: options),
					static (state, it, grammars) =>
						new SatisfiesConstraint<T>(
							it,
							grammars,
							state.Predicate,
							state.DoNotPopulateThisValue.TrimCommonWhiteSpace(),
							state.Options).Invert()),
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
			IAsyncContextConstraint<T>
	{
		private Exception? _exception;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => _exception;

		public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome outcome =
				await options.CheckRepeatedly(_ => Task.FromResult(IsMet(actual, cancellationToken)), context);
			if (outcome == Outcome.Undecided)
			{
				Outcome = Outcome.Undecided;
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
				// A predicate that threw answered nothing, so it fails the expectation and its negation alike.
				_exception = exception;
				Outcome = Outcome.FailureBothWays;
				return false;
			}

			_exception = null;
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
				stringBuilder.Append("the predicate did throw ").Append(_exception.FormatForMessage(indentation));
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
