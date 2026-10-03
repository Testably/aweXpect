using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect;

public static partial class ThatSignaler
{
	private const string Times = " times";

	/// <summary>
	///     Verifies that the expected callback was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions options = new();
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint(it, grammars, state.Quantifier, state.Options)),
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
		return new SignalCountWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, state.Quantifier, state.Options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignaledResult Signaled(
		this IThat<Signaler> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions options = new();
		return new SignaledResult(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint(it, grammars, state.Quantifier, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignaledWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions<TParameter> options = new();
		return new SignaledWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, state.Quantifier, state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from <see cref="DidNotSignalResult.Within(TimeSpan)" /> before
	///     it can succeed, and fails as soon as the callback is signaled. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions options = new();
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint(it, grammars, state.Quantifier, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from
	///     <see cref="DidNotSignalResult{TParameter}.Within(TimeSpan)" /> before it can succeed, and fails as soon as a
	///     matching signal is received. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions<TParameter> options = new();
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, state.Quantifier, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled fewer than <paramref name="times" /> times.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from <see cref="DidNotSignalResult.Within(TimeSpan)" /> before
	///     it can succeed, and fails as soon as the callback was signaled the given number of
	///     <paramref name="times" />. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions options = new();
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint(it, grammars, state.Quantifier, state.Options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled fewer than
	///     <paramref name="times" /> times.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from
	///     <see cref="DidNotSignalResult{TParameter}.Within(TimeSpan)" /> before it can succeed, and fails as soon as
	///     the callback was signaled with a matching parameter the given number of <paramref name="times" />. Without a
	///     timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions<TParameter> options = new();
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(
				(Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, state.Quantifier, state.Options).Invert()),
			subject,
			options);
	}

	private static void AppendNormalCallbackExpectation(StringBuilder stringBuilder, Quantifier quantifier,
		SignalerOptions options, TimeSpan? defaultTimeout)
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
		if (defaultTimeout is not null && defaultTimeout != Timeout.InfiniteTimeSpan)
		{
			stringBuilder.Append(" within ");
			Formatter.Format(stringBuilder, defaultTimeout.Value);
		}
	}

	private static void AppendNegatedCallbackExpectation(StringBuilder stringBuilder, Quantifier quantifier,
		SignalerOptions options, TimeSpan? defaultTimeout)
	{
		// Rendering the complementary quantifier makes e.g. DidNotSignal() read like Signaled().Never().
		quantifier.Negate();
		AppendNormalCallbackExpectation(stringBuilder, quantifier, options, defaultTimeout);
		quantifier.Negate();
	}

	/// <summary>
	///     Returns the <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />
	///     when the wait falls back to it, so that the expectation can show it.
	/// </summary>
	private static TimeSpan? GetDefaultTimeout(SignalerOptions options, int determinableAmount)
		=> options.Timeout is null && determinableAmount > 0
			? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()
			: null;

	/// <remarks>
	///     The event recording phrases its occurrences the same way.<br />
	///     <c>only</c> says that too few occurred, so it is left out when fewer would also meet the
	///     <paramref name="quantifier" />, e.g. for <c>not exactly once</c>.
	/// </remarks>
	internal static void AppendOccurrences(StringBuilder stringBuilder, Quantifier quantifier, int count)
	{
		if (count == 0)
		{
			stringBuilder.Append("never recorded");
			return;
		}

		if (quantifier.Check(count, false) is null && !(quantifier.Check(0, true) ?? quantifier.IsNegated))
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

	/// <remarks>
	///     <see cref="Signaler.Wait(TimeSpan?, CancellationToken)" /> ends at the cancellation like at its timeout, but
	///     the signals received until then decide nothing, unless the cancellation counts as the
	///     <paramref name="timeout" /> having elapsed.
	/// </remarks>
	private static bool IsCanceledBeforeTheTimeout(bool isSuccess, TimeSpan? timeout, TimeSpan waited,
		IEvaluationContext context)
		=> !isSuccess && timeout != TimeSpan.Zero && context.Cancellation.Token.IsCancellationRequested &&
		   !(timeout is { } t && context.Cancellation.HasWaitElapsed(t, waited));

	private static void AppendWaitedTime(StringBuilder stringBuilder, TimeSpan? waitedTime, bool? isSuccess)
	{
		if (waitedTime is null)
		{
			return;
		}

		// A successful wait stopped as soon as enough signals were received, otherwise the timeout expired.
		stringBuilder.Append(isSuccess == true ? " after " : " within ");
		Formatter.Format(stringBuilder, waitedTime.Value);
	}

	private sealed class SignaledConstraint(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		SignalerOptions options)
		: ConstraintResult.WithNotNullValue<SignalerResult>(it, grammars), IAsyncContextConstraint<Signaler>
	{
		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

		public async Task<ConstraintResult> IsMetBy(Signaler actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			int determinableAmount = quantifier.DeterminableAmount;
			_defaultTimeout = GetDefaultTimeout(options, determinableAmount);
			TimeSpan? timeout = determinableAmount > 0 ? options.Timeout ?? _defaultTimeout : TimeSpan.Zero;
			Stopwatch stopwatch = Stopwatch.StartNew();
			// With nothing to wait for, the timeout is zero, and the Times overload rejects an amount of zero.
			Actual = determinableAmount > 0
				? await actual.WaitAsync(determinableAmount.Times(), timeout, cancellationToken)
				: await actual.WaitAsync(timeout, cancellationToken);
			TimeSpan waited = stopwatch.Elapsed;
			_waitedTime = options.Timeout is null && _defaultTimeout is null ? null : waited;
			if (IsCanceledBeforeTheTimeout(Actual.IsSuccess, timeout, waited, context))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			Outcome = quantifier.Check(Actual.Count, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalCallbackExpectation(stringBuilder, quantifier, options, _defaultTimeout);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, quantifier, Actual?.Count ?? 0);
			AppendWaitedTime(stringBuilder, _waitedTime, Actual?.IsSuccess);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNegatedCallbackExpectation(stringBuilder, quantifier, options, _defaultTimeout);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}

	private sealed class SignaledConstraint<TParameter>(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier,
		SignalerOptions<TParameter> options)
		: ConstraintResult.WithNotNullValue<SignalerResult<TParameter>>(it, grammars),
			IAsyncContextConstraint<Signaler<TParameter>>
	{
		private int _actualCount;
		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

		public async Task<ConstraintResult> IsMetBy(
			Signaler<TParameter> actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			SignalerOptions<TParameter> o = options;
			int determinableAmount = quantifier.DeterminableAmount;
			_defaultTimeout = GetDefaultTimeout(options, determinableAmount);
			TimeSpan? timeout = determinableAmount > 0 ? options.Timeout ?? _defaultTimeout : TimeSpan.Zero;
			Stopwatch stopwatch = Stopwatch.StartNew();
			// With nothing to wait for, the timeout is zero, and the Times overload rejects an amount of zero.
			Actual = await UserCode.InvokeAsync(async () => determinableAmount > 0
				? await actual.WaitAsync(determinableAmount.Times(), o.Matches, timeout, cancellationToken)
				: await actual.WaitAsync(o.Matches, timeout, cancellationToken), "the predicate", CancellationToken.None);
			TimeSpan waited = stopwatch.Elapsed;
			_waitedTime = o.Timeout is null && _defaultTimeout is null ? null : waited;
			if (IsCanceledBeforeTheTimeout(Actual.IsSuccess, timeout, waited, context))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			_actualCount = Actual.Parameters.Count(p => UserCode.Invoke(o.Matches, p, "the predicate"));

			Outcome = quantifier.Check(_actualCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalCallbackExpectation(stringBuilder, quantifier, options, _defaultTimeout);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, quantifier, _actualCount);

			if (Actual?.Count > 0)
			{
				stringBuilder.Append(" in ");
				ValueFormatters.Format(Formatter, stringBuilder, Actual.Parameters, FormattingOptions.MultipleLines);
			}

			AppendWaitedTime(stringBuilder, _waitedTime, Actual?.IsSuccess);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNegatedCallbackExpectation(stringBuilder, quantifier, options, _defaultTimeout);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
