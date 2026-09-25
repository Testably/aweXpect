using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	/// <remarks>
	///     The expectation waits for the full timeout from <see cref="DidNotSignalResult.Within(TimeSpan)" /> before
	///     it can succeed, and fails as soon as the callback is signaled. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions options = new();
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options).Invert()),
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
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject)
	{
		Quantifier quantifier = new();
		SignalerOptions<TParameter> options = new();
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options).Invert()),
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
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions options = new();
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, quantifier, options).Invert()),
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
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> subject,
		Times times)
	{
		Quantifier quantifier = new();
		quantifier.AtLeast(times.Value);
		SignalerOptions<TParameter> options = new();
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, quantifier, options).Invert()),
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
	///     Returns the <see cref="Customization.AwexpectCustomization.SettingsCustomizationValue.DefaultSignalerTimeout" />
	///     when the wait falls back to it, so that the expectation can show it.
	/// </summary>
	private static TimeSpan? GetDefaultTimeout(SignalerOptions options, int determinableAmount)
		=> options.Timeout is null && determinableAmount > 0
			? Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()
			: null;

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

	/// <remarks>
	///     <see cref="Signaler.Wait(TimeSpan?, CancellationToken)" /> ends at the cancellation like at its timeout, but
	///     the signals received until then decide nothing.
	/// </remarks>
	private static void ThrowIfTheWaitWasCanceled(bool isSuccess, TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		if (!isSuccess && timeout != TimeSpan.Zero)
		{
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

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
		: ConstraintResult.WithNotNullValue<SignalerResult>(it, grammars), IAsyncConstraint<Signaler>
	{
		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

		public async Task<ConstraintResult> IsMetBy(Signaler actual, CancellationToken cancellationToken)
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
			// A single signal must not be awaited through the Times overload: it leaves the signaler with a disposed
			// CountdownEvent, so that any later Signal() would throw an ObjectDisposedException.
			Actual = await Task.Run(() =>
				{
					// Measured inside the task, so that a busy thread pool does not count as waited time.
					Stopwatch stopwatch = Stopwatch.StartNew();
					SignalerResult result = determinableAmount > 1
						? actual.Wait(determinableAmount.Times(), timeout, cancellationToken)
						: actual.Wait(timeout, cancellationToken);
					_waitedTime = options.Timeout is null && _defaultTimeout is null ? null : stopwatch.Elapsed;
					return result;
				},
				CancellationToken.None);
			ThrowIfTheWaitWasCanceled(Actual.IsSuccess, timeout, cancellationToken);

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
			IAsyncConstraint<Signaler<TParameter>>
	{
		private int _actualCount;
		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

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
			int determinableAmount = quantifier.DeterminableAmount;
			_defaultTimeout = GetDefaultTimeout(options, determinableAmount);
			TimeSpan? timeout = determinableAmount > 0 ? options.Timeout ?? _defaultTimeout : TimeSpan.Zero;
			// A single signal must not be awaited through the Times overload: it leaves the signaler with a disposed
			// CountdownEvent, so that any later Signal() would throw an ObjectDisposedException.
			Actual = await Task.Run(() =>
				{
					// Measured inside the task, so that a busy thread pool does not count as waited time.
					Stopwatch stopwatch = Stopwatch.StartNew();
					SignalerResult<TParameter> result = UserCode.Invoke(() => determinableAmount > 1
						? actual.Wait(determinableAmount.Times(), o.Matches, timeout, cancellationToken)
						: actual.Wait(o.Matches, timeout, cancellationToken), "the predicate");
					_waitedTime = o.Timeout is null && _defaultTimeout is null ? null : stopwatch.Elapsed;
					return result;
				},
				CancellationToken.None);
			ThrowIfTheWaitWasCanceled(Actual.IsSuccess, timeout, cancellationToken);

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
