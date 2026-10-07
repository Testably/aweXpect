using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Internal;

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
	///     When the <paramref name="reason" /> is <see langword="null" />, empty or only whitespace, it is ignored.
	/// </remarks>
	public ExpectationResult Because(string? reason)
	{
		if (!string.IsNullOrWhiteSpace(reason))
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
	///     When the <paramref name="reason" /> resolves to <see langword="null" />, empty or only whitespace, it is ignored.
	///     A <paramref name="reason" /> that is still pending when the timeout elapses or the evaluation is canceled is
	///     not awaited any longer, and the failure message states that the reason was not available in time.
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
	///         Customize.aweXpect.Global.Settings().TestCancellation
	///         .Set(TestCancellation.FromCancellationToken(() => cancellationToken))
	///     </c>
	///     to apply the <paramref name="cancellationToken" /> globally.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A cancellation token is already set.</exception>
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
	///         Customize.aweXpect.Global.Settings().TestCancellation
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
		if (result.Outcome == Outcome.Success && !ExpectationBuilder.IsTracing)
		{
			return Task.CompletedTask.GetAwaiter();
		}

		return GetResultOrThrow(new ValueTask<ConstraintResult>(result)).GetAwaiter();
	}

	/// <inheritdoc />
	/// <remarks>
	///     The combination does not trace its members, so a met expectation is traced here.
	/// </remarks>
	internal override async Task<Result> GetResult(int index)
	{
		ConstraintResult result = await ExpectationBuilder.IsMet(false);
		if (result.Outcome == Outcome.Success && ExpectationBuilder.IsTracing)
		{
			Tracing.WriteSuccess(ExpectationBuilder.Subject, result);
		}

		return new Result(index + 1, ExpectationBuilder.Subject, result, true);
	}

	/// <inheritdoc />
	internal override Task EndEvaluation()
		=> ExpectationBuilder.EndEvaluation();

	/// <inheritdoc />
	internal override Task ResolvePendingReasons()
		=> ExpectationBuilder.ResolvePendingReasons();

	/// <inheritdoc />
	internal override void UseTimeSystem(ITimeSystem timeSystem)
		=> ExpectationBuilder.UseTimeSystem(timeSystem);

	/// <inheritdoc />
	internal override void AddRemainingCancellations(List<EvaluationCancellation> cancellations)
		=> cancellations.Add(ExpectationBuilder.GetRemainingCancellation());

	private async Task GetResultOrThrow(ValueTask<ConstraintResult> isMet)
	{
		ConstraintResult result = await isMet;

		if (result.Outcome == Outcome.Success)
		{
			Tracing.WriteSuccess(ExpectationBuilder.Subject, result);
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
	///     When the <paramref name="reason" /> is <see langword="null" />, empty or only whitespace, it is ignored.
	/// </remarks>
	public TSelf Because(string? reason)
	{
		if (!string.IsNullOrWhiteSpace(reason))
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
	///     When the <paramref name="reason" /> resolves to <see langword="null" />, empty or only whitespace, it is ignored.
	///     A <paramref name="reason" /> that is still pending when the timeout elapses or the evaluation is canceled is
	///     not awaited any longer, and the failure message states that the reason was not available in time.
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
	///     <para />
	///     A <see cref="ValueTaskAwaiter{TResult}" />, so that returning the value right away allocates no task. Every
	///     other evaluation is backed by a <see cref="Task{TResult}" />, which <c>GetAwaiter().GetResult()</c> can block
	///     on.
	/// </remarks>
	[StackTraceHidden]
	public ValueTaskAwaiter<TType> GetAwaiter()
		=> TryGetValueRightAway(out TType value, out Task<TType>? evaluation)
			? new ValueTask<TType>(value).GetAwaiter()
			: new ValueTask<TType>(evaluation!).GetAwaiter();

	/// <summary>
	///     Evaluates the expectations, see <see cref="GetAwaiter" />.
	/// </summary>
	/// <returns>
	///     <see langword="true" />, when the expectation was met right away and the <paramref name="value" /> is set, and
	///     <see langword="false" /> with the <paramref name="evaluation" /> otherwise.
	/// </returns>
	[StackTraceHidden]
	internal bool TryGetValueRightAway(out TType value, out Task<TType>? evaluation)
	{
		value = default!;
		ValueTask<ConstraintResult> isMet = ExpectationBuilder.IsMet();
		if (!isMet.IsCompletedSuccessfully)
		{
			evaluation = GetResultOrThrow(isMet);
			return false;
		}

		ConstraintResult result = isMet.Result;
		if (result.Outcome == Outcome.Success && !ExpectationBuilder.IsTracing)
		{
			value = GetStoredValueOrDefault(result);
			evaluation = null;
			return true;
		}

		evaluation = GetResultOrThrow(new ValueTask<ConstraintResult>(result));
		return false;
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
	///         Customize.aweXpect.Global.Settings().TestCancellation
	///         .Set(TestCancellation.FromCancellationToken(() => cancellationToken))
	///     </c>
	///     to apply the <paramref name="cancellationToken" /> globally.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A cancellation token is already set.</exception>
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
	///         Customize.aweXpect.Global.Settings().TestCancellation
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
	/// <remarks>
	///     The combination does not trace its members, so a met expectation is traced here.
	/// </remarks>
	internal override async Task<Result> GetResult(int index)
	{
		ConstraintResult result = await ExpectationBuilder.IsMet(false);
		if (result.Outcome == Outcome.Success && ExpectationBuilder.IsTracing)
		{
			Tracing.WriteSuccess(ExpectationBuilder.Subject, result);
		}

		return new Result(index + 1, ExpectationBuilder.Subject, result, true);
	}

	/// <inheritdoc />
	internal override Task EndEvaluation()
		=> ExpectationBuilder.EndEvaluation();

	/// <inheritdoc />
	internal override Task ResolvePendingReasons()
		=> ExpectationBuilder.ResolvePendingReasons();

	/// <inheritdoc />
	internal override void UseTimeSystem(ITimeSystem timeSystem)
		=> ExpectationBuilder.UseTimeSystem(timeSystem);

	/// <inheritdoc />
	internal override void AddRemainingCancellations(List<EvaluationCancellation> cancellations)
		=> cancellations.Add(ExpectationBuilder.GetRemainingCancellation());

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
			case Outcome.Success:
				Tracing.WriteSuccess(ExpectationBuilder.Subject, result);
				return GetStoredValueOrDefault(result);
			case Outcome.Undecided:
				Fail.Inconclusive(await FromFailure(result));
				break;
			case Outcome.Failure:
			case Outcome.FailureBothWays:
				Fail.Test(await FromFailure(result), result.FailureCause);
				break;
		}

		throw Tracing.WriteException(
			new FailException($"The outcome {result.Outcome} of {Formatter.Format(result.GetType())} is not supported."));
	}

	/// <summary>
	///     The stored value of the met <paramref name="result" />.
	/// </summary>
	/// <remarks>
	///     A met expectation can be without a <typeparamref name="TType" />, e.g. when another alternative of an
	///     <c>Or</c> was met than the one that determines the type. It is then the <see langword="default" /> value.
	/// </remarks>
	private static TType GetStoredValueOrDefault(ConstraintResult result)
		=> result.TryGetStoredValue(out TType? value) ? value! : default!;
}
