using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.TimeSystem;
using aweXpect.Customization;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation without an underlying value.
/// </summary>
[StackTraceHidden]
public class ExpectationResult(ExpectationBuilder expectationBuilder)
	: Expectation, IOptionsProvider<ExpectationBuilder>
{
	/// <summary>
	///     The <see cref="Core.ExpectationBuilder" /> of the expectation.
	/// </summary>
	protected ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	/// <inheritdoc cref="IOptionsProvider{ExpectationBuilder}.Options" />
	ExpectationBuilder IOptionsProvider<ExpectationBuilder>.Options => ExpectationBuilder;

	/// <inheritdoc cref="object.ToString()" />
	public override string? ToString()
		=> ExpectationBuilder.ToString();

	/// <summary>
	///     Provide a <paramref name="reason" /> explaining why the constraint is needed.<br />
	///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
	/// </summary>
	/// <remarks>
	///     When the <paramref name="reason" /> is <see langword="null" /> or empty, it is ignored.
	/// </remarks>
	public ExpectationResult Because(string? reason)
	{
		if (!string.IsNullOrEmpty(reason))
		{
			ExpectationBuilder.AddReason(reason);
		}

		return this;
	}

	/// <summary>
	///     Provide an <see langword="async" /> <paramref name="reason" /> explaining why the constraint is needed.<br />
	///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
	/// </summary>
	/// <remarks>
	///     When the <paramref name="reason" /> resolves to <see langword="null" /> or empty, it is ignored.
	/// </remarks>
	public ExpectationResult Because(Task<string?> reason)
	{
		ExpectationBuilder.AddReason(reason);
		return this;
	}

	/// <summary>
	///     Sets the <see cref="CancellationToken" /> to be passed to expectations.
	/// </summary>
	/// <remarks>
	///     An awaited task, such as a <see cref="Task{TResult}" /> subject or the task of an asynchronous delegate, is
	///     abandoned when the <paramref name="cancellationToken" /> is canceled before it completes. An expectation that
	///     is interrupted by the cancellation is inconclusive: it could not be verified, because the evaluation was
	///     already canceled.
	///     <para />
	///     Use
	///     <c>
	///         Customize.aweXpect.Settings().TestCancellation
	///         .Set(TestCancellation.FromCancellationToken(() => cancellationToken))
	///     </c>
	///     to apply the <paramref name="cancellationToken" /> globally.
	/// </remarks>
	public ExpectationResult WithCancellation(CancellationToken cancellationToken)
	{
		ExpectationBuilder.WithCancellation(cancellationToken);
		return this;
	}

	/// <summary>
	///     Sets the <paramref name="timeout" /> to be passed to expectations.
	/// </summary>
	/// <remarks>
	///     An awaited task, such as a <see cref="Task{TResult}" /> subject or the task of an asynchronous delegate, is
	///     abandoned when the <paramref name="timeout" /> elapses before it completes, and the expectation fails with
	///     <c>did not finish within …</c>.
	///     <para />
	///     The tightest limit wins: a longer <paramref name="timeout" /> does not loosen an earlier one, the limit of
	///     the expectation itself (e.g. <c>ExecutesIn().AtMost(…)</c>) or the global timeout.
	///     <see cref="Timeout.InfiniteTimeSpan" /> imposes no limit.
	///     <para />
	///     Use
	///     <c>
	///         Customize.aweXpect.Settings().TestCancellation
	///         .Set(TestCancellation.FromTimeout(timeout))
	///     </c>
	///     to apply the <paramref name="timeout" /> globally.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public ExpectationResult WithTimeout(TimeSpan timeout)
	{
		ExpectationBuilder.WithTimeout(timeout);
		return this;
	}

	/// <summary>
	///     By awaiting the result, the expectations are verified.
	///     <para />
	///     Will throw an exception, when the expectations are not met.
	/// </summary>
	/// <remarks>
	///     An expectation that is met without waiting for anything completes right away, as starting the asynchronous
	///     evaluation would take longer than the expectation itself.
	/// </remarks>
	public TaskAwaiter GetAwaiter()
	{
		ValueTask<ConstraintResult> isMet = ExpectationBuilder.IsMet();
		if (!isMet.IsCompletedSuccessfully)
		{
			return GetResultOrThrow(isMet).GetAwaiter();
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome == Outcome.Success && Customize.aweXpect.TraceWriter is null)
		{
			return Task.CompletedTask.GetAwaiter();
		}

		return GetResultOrThrow(new ValueTask<ConstraintResult>(result)).GetAwaiter();
	}

	/// <inheritdoc />
	internal override async Task<Result> GetResult(int index)
		=> new(index + 1, ExpectationBuilder.Subject, await ExpectationBuilder.IsMet(), true);

	/// <inheritdoc />
	internal override Task EndEvaluation()
		=> ExpectationBuilder.EndEvaluation();

	/// <summary>
	///     Specifies a <see cref="ITimeSystem" /> to use for the expectation.
	/// </summary>
	internal ExpectationResult UseTimeSystem(ITimeSystem timeSystem)
	{
		ExpectationBuilder.UseTimeSystem(timeSystem);
		return this;
	}

	private async Task GetResultOrThrow(ValueTask<ConstraintResult> isMet)
	{
		ConstraintResult result = await isMet;

		if (result.Outcome == Outcome.Success)
		{
			ITraceWriter? traceWriter = Customize.aweXpect.TraceWriter;
			if (traceWriter != null)
			{
				StringBuilder sb = new();
				sb.Append("  Successfully verified that ");
				sb.Append(result.TryGetValue(out IDescribableSubject? describableSubject)
					? describableSubject.GetDescription()
					: ExpectationBuilder.Subject);
				sb.Append(' ');
				result.AppendExpectation(sb);
				traceWriter.WriteMessage(sb.ToString());
			}

			return;
		}

		if (result.Outcome == Outcome.Undecided)
		{
			Fail.Inconclusive(await FromFailure(result));
		}

		Fail.Test(await FromFailure(result), result.FailureCause);
	}

	/// <summary>
	///     Creates the exception message from the <paramref name="failure" /> and ends the evaluation afterward.
	/// </summary>
	private async Task<string> FromFailure(ConstraintResult failure)
	{
		try
		{
			return await ExpectationBuilder.FromFailure(failure);
		}
		finally
		{
			await ExpectationBuilder.EndEvaluation();
		}
	}
}

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
/// </summary>
public class ExpectationResult<TType>(ExpectationBuilder expectationBuilder)
	: ExpectationResult<TType, ExpectationResult<TType>>(expectationBuilder);

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
/// </summary>
[StackTraceHidden]
public class ExpectationResult<TType, TSelf>(ExpectationBuilder expectationBuilder)
	: Expectation,
		IOptionsProvider<ExpectationBuilder>
	where TSelf : ExpectationResult<TType, TSelf>
{
	/// <summary>
	///     The <see cref="Core.ExpectationBuilder" /> of the expectation.
	/// </summary>
	protected ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	/// <inheritdoc cref="IOptionsProvider{ExpectationBuilder}.Options" />
	ExpectationBuilder IOptionsProvider<ExpectationBuilder>.Options => ExpectationBuilder;

	/// <inheritdoc cref="object.ToString()" />
	public override string? ToString()
		=> ExpectationBuilder.ToString();

	/// <summary>
	///     Provide a <paramref name="reason" /> explaining why the constraint is needed.<br />
	///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
	/// </summary>
	/// <remarks>
	///     When the <paramref name="reason" /> is <see langword="null" /> or empty, it is ignored.
	/// </remarks>
	public TSelf Because(string? reason)
	{
		if (!string.IsNullOrEmpty(reason))
		{
			ExpectationBuilder.AddReason(reason);
		}

		return (TSelf)this;
	}

	/// <summary>
	///     Provide an <see langword="async" /> <paramref name="reason" /> explaining why the constraint is needed.<br />
	///     If the phrase does not start with the word <i>because</i>, it is prepended automatically.
	/// </summary>
	/// <remarks>
	///     When the <paramref name="reason" /> resolves to <see langword="null" /> or empty, it is ignored.
	/// </remarks>
	public TSelf Because(Task<string?> reason)
	{
		ExpectationBuilder.AddReason(reason);
		return (TSelf)this;
	}

	/// <summary>
	///     By awaiting the result, the expectations are verified.
	///     <para />
	///     Will throw an exception, when the expectations are not met.<br />
	///     Otherwise, it will return the <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     An expectation that is met without waiting for anything returns its value right away, as starting the
	///     asynchronous evaluation would take longer than the expectation itself.
	/// </remarks>
	[StackTraceHidden]
	public TaskAwaiter<TType> GetAwaiter()
	{
		ValueTask<ConstraintResult> isMet = ExpectationBuilder.IsMet();
		if (!isMet.IsCompletedSuccessfully)
		{
			return GetResultOrThrow(isMet).GetAwaiter();
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome == Outcome.Success && Customize.aweXpect.TraceWriter is null &&
		    result.TryGetStoredValue(out TType? value))
		{
			return Task.FromResult(value!).GetAwaiter();
		}

		return GetResultOrThrow(new ValueTask<ConstraintResult>(result)).GetAwaiter();
	}

	/// <summary>
	///     Sets the <paramref name="cancellationToken" /> to be passed to expectations.
	/// </summary>
	/// <remarks>
	///     An awaited task, such as a <see cref="Task{TResult}" /> subject or the task of an asynchronous delegate, is
	///     abandoned when the <paramref name="cancellationToken" /> is canceled before it completes. An expectation that
	///     is interrupted by the cancellation is inconclusive: it could not be verified, because the evaluation was
	///     already canceled.
	///     <para />
	///     Use
	///     <c>
	///         Customize.aweXpect.Settings().TestCancellation
	///         .Set(TestCancellation.FromCancellationToken(() => cancellationToken))
	///     </c>
	///     to apply the <paramref name="cancellationToken" /> globally.
	/// </remarks>
	public TSelf WithCancellation(CancellationToken cancellationToken)
	{
		ExpectationBuilder.WithCancellation(cancellationToken);
		return (TSelf)this;
	}

	/// <summary>
	///     Sets the <paramref name="timeout" /> to be passed to expectations.
	/// </summary>
	/// <remarks>
	///     An awaited task, such as a <see cref="Task{TResult}" /> subject or the task of an asynchronous delegate, is
	///     abandoned when the <paramref name="timeout" /> elapses before it completes, and the expectation fails with
	///     <c>did not finish within …</c>.
	///     <para />
	///     The tightest limit wins: a longer <paramref name="timeout" /> does not loosen an earlier one, the limit of
	///     the expectation itself (e.g. <c>ExecutesIn().AtMost(…)</c>) or the global timeout.
	///     <see cref="Timeout.InfiniteTimeSpan" /> imposes no limit.
	///     <para />
	///     Use
	///     <c>
	///         Customize.aweXpect.Settings().TestCancellation
	///         .Set(TestCancellation.FromTimeout(timeout))
	///     </c>
	///     to apply the <paramref name="timeout" /> globally.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public TSelf WithTimeout(TimeSpan timeout)
	{
		ExpectationBuilder.WithTimeout(timeout);
		return (TSelf)this;
	}

	/// <inheritdoc />
	internal override async Task<Result> GetResult(int index)
		=> new(index + 1, ExpectationBuilder.Subject, await ExpectationBuilder.IsMet(), true);

	/// <inheritdoc />
	internal override Task EndEvaluation()
		=> ExpectationBuilder.EndEvaluation();

	/// <summary>
	///     Specifies a <see cref="ITimeSystem" /> to use for the expectation.
	/// </summary>
	internal TSelf UseTimeSystem(ITimeSystem timeSystem)
	{
		ExpectationBuilder.UseTimeSystem(timeSystem);
		return (TSelf)this;
	}

	/// <inheritdoc cref="ExpectationResult.FromFailure(ConstraintResult)" />
	private async Task<string> FromFailure(ConstraintResult failure)
	{
		try
		{
			return await ExpectationBuilder.FromFailure(failure);
		}
		finally
		{
			await ExpectationBuilder.EndEvaluation();
		}
	}

	[StackTraceHidden]
	private async Task<TType> GetResultOrThrow(ValueTask<ConstraintResult> isMet)
	{
		ConstraintResult result = await isMet;

		switch (result.Outcome)
		{
			case Outcome.Success
				when result.TryGetStoredValue(out TType? value):
				ITraceWriter? traceWriter = Customize.aweXpect.TraceWriter;
				if (traceWriter != null)
				{
					StringBuilder sb = new();
					sb.Append("  Successfully verified that ");
					sb.Append(result.TryGetValue(out IDescribableSubject? describableSubject)
						? describableSubject.GetDescription()
						: ExpectationBuilder.Subject);
					sb.Append(' ');
					result.AppendExpectation(sb);
					traceWriter.WriteMessage(sb.ToString());
				}

				return value!;
			case Outcome.Undecided:
				Fail.Inconclusive(await FromFailure(result));
				break;
			case Outcome.Failure:
			case Outcome.FailureBothWays:
				Fail.Test(await FromFailure(result), result.FailureCause);
				break;
		}

		throw Tracing.WriteException(
			new FailException(
				$"The value in {Formatter.Format(result.GetType())} did not match expected type {Formatter.Format(typeof(TType))}."));
	}
}
