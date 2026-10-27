using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect;

public static partial class ThatSignaler
{
	/// <summary>
	///     Verifies that the expected callback was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler?> subject)
	{
		return new SignalCountResult(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default,
				static (quantifier, it, grammars)
					=> new SignaledConstraint(it, grammars, quantifier),
				out SignaledConstraint constraint),
			subject,
			constraint,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>?> subject)
	{
		return new SignalCountWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default,
				static (quantifier, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, quantifier),
				out SignaledConstraint<TParameter> constraint),
			subject,
			constraint,
			constraint,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignaledResult Signaled(
		this IThat<Signaler?> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		return new SignaledResult(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default.AtLeast(times.Value),
				static (quantifier, it, grammars)
					=> new SignaledConstraint(it, grammars, quantifier),
				out SignaledConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignaledWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>?> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		return new SignaledWhoseResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default.AtLeast(times.Value),
				static (quantifier, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, quantifier),
				out SignaledConstraint<TParameter> constraint),
			subject,
			constraint,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from <see cref="SignalerOptionsExtensions.Within{TSelf}(TSelf, TimeSpan)" /> before
	///     it can succeed, and fails as soon as the callback is signaled. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler?> subject)
	{
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default,
				static (quantifier, it, grammars)
					=> new SignaledConstraint(it, grammars, quantifier).Invert(),
				out SignaledConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from
	///     <see cref="SignalerOptionsExtensions.Within{TSelf}(TSelf, TimeSpan)" /> before it can succeed, and fails as soon as a
	///     matching signal is received. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>?> subject)
	{
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default,
				static (quantifier, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, quantifier).Invert(),
				out SignaledConstraint<TParameter> constraint),
			subject,
			constraint,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled fewer than <paramref name="times" /> times.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from <see cref="SignalerOptionsExtensions.Within{TSelf}(TSelf, TimeSpan)" /> before
	///     it can succeed, and fails as soon as the callback was signaled the given number of
	///     <paramref name="times" />. Without a timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult DidNotSignal(
		this IThat<Signaler?> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		return new DidNotSignalResult(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default.AtLeast(times.Value),
				static (quantifier, it, grammars)
					=> new SignaledConstraint(it, grammars, quantifier).Invert(),
				out SignaledConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled fewer than
	///     <paramref name="times" /> times.
	/// </summary>
	/// <remarks>
	///     The expectation waits for the full timeout from
	///     <see cref="SignalerOptionsExtensions.Within{TSelf}(TSelf, TimeSpan)" /> before it can succeed, and fails as soon as
	///     the callback was signaled with a matching parameter the given number of <paramref name="times" />. Without a
	///     timeout, it waits for the
	///     <see cref="Customization.AwexpectCustomization.SettingsCustomization.DefaultSignalerTimeout" />, which
	///     is 30 seconds unless customized. Canceling the evaluation ends the wait early and leaves the expectation
	///     inconclusive.
	/// </remarks>
	[GuaranteesNotNull]
	public static DidNotSignalResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>?> subject,
		Times times)
	{
		ThrowHelper.ThrowIfTimesIsNotPositive(times);
		return new DidNotSignalResult<TParameter>(subject.Get().ExpectationBuilder.AddConstraint(Quantifier.Default.AtLeast(times.Value),
				static (quantifier, it, grammars)
					=> new SignaledConstraint<TParameter>(it, grammars, quantifier).Invert(),
				out SignaledConstraint<TParameter> constraint),
			subject,
			constraint,
			constraint);
	}

	/// <remarks>
	///     The negated expectation reads as the complementary quantifier, e.g. <c>DidNotSignal()</c> like
	///     <c>Signaled().Never()</c>.
	/// </remarks>
	private static void AppendCallbackExpectation(StringBuilder stringBuilder, Quantifier quantifier, bool isNegated,
		object? predicates, SignalerOptions options, TimeSpan? defaultTimeout)
	{
		if (quantifier.IsNever(isNegated))
		{
			stringBuilder.Append("has never recorded the callback");
		}
		else
		{
			stringBuilder.Append("has recorded the callback ").Append(quantifier.ToString(isNegated));
		}

		stringBuilder.Append(predicates).Append(options);
		if (defaultTimeout is not null && defaultTimeout != Timeout.InfiniteTimeSpan)
		{
			stringBuilder.Append(" within ");
			Formatter.Format(stringBuilder, defaultTimeout.Value);
		}
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
	internal static void AppendOccurrences(StringBuilder stringBuilder, Quantifier quantifier, bool isNegated,
		int count)
	{
		if (count == 0)
		{
			stringBuilder.Append("never recorded");
			return;
		}

		if (quantifier.Check(count, false) is null && !(quantifier.Check(0, true, isNegated) ?? isNegated))
		{
			stringBuilder.Append("only ");
		}

		stringBuilder.Append("recorded ").AppendOccurrences(count);
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
		Quantifier quantifier)
		: ConstraintResult.WithNotNullValue<SignalerResult>(it, grammars), IAsyncContextConstraint<Signaler?>,
			IOptionsHolderSource<Quantifier>,
			IOptionsHolderSource<SignalerOptions>
	{
		private OptionsHolder<Quantifier>? _quantifierHolder;

		private Quantifier CurrentQuantifier => _quantifierHolder is null ? quantifier : _quantifierHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<Quantifier> IOptionsHolderSource<Quantifier>.Holder
			=> _quantifierHolder ??= new OptionsHolder<Quantifier>(quantifier);

		private OptionsHolder<SignalerOptions>? _optionsHolder;

		private SignalerOptions CurrentOptions => _optionsHolder is null ? SignalerOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<SignalerOptions> IOptionsHolderSource<SignalerOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<SignalerOptions>(SignalerOptions.Default);

		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

		public async ValueTask<ConstraintResult> IsMetBy(Signaler? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			if (actual == null)
			{
				Actual = null;
				_defaultTimeout = null;
				Outcome = Outcome.Failure;
				return this;
			}

			int determinableAmount = CurrentQuantifier.DeterminableAmount;
			_defaultTimeout = GetDefaultTimeout(CurrentOptions, determinableAmount);
			TimeSpan? timeout = determinableAmount > 0 ? CurrentOptions.Timeout ?? _defaultTimeout : TimeSpan.Zero;
			long startTimestamp = context.GetTimestamp();
			// With nothing to wait for, the timeout is zero, and the Times overload rejects an amount of zero.
			Actual = determinableAmount > 0
				? await context.WaitForSignalsAsync(actual, determinableAmount.Times(), timeout, cancellationToken)
				: await actual.WaitAsync(timeout, cancellationToken);
			TimeSpan waited = context.GetElapsedTime(startTimestamp);
			_waitedTime = CurrentOptions.Timeout is null && _defaultTimeout is null ? null : waited;
			if (IsCanceledBeforeTheTimeout(Actual.IsSuccess, timeout, waited, context))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			Outcome = CurrentQuantifier.Check(Actual.Count, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCallbackExpectation(stringBuilder, CurrentQuantifier, false, null, CurrentOptions, _defaultTimeout);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, CurrentQuantifier, IsNegated, Actual?.Count ?? 0);
			AppendWaitedTime(stringBuilder, _waitedTime, Actual?.IsSuccess);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCallbackExpectation(stringBuilder, CurrentQuantifier, true, null, CurrentOptions, _defaultTimeout);
	}

	private sealed class SignaledConstraint<TParameter>(
		string it,
		ExpectationGrammars grammars,
		Quantifier quantifier)
		: ConstraintResult.WithNotNullValue<SignalerResult<TParameter>>(it, grammars),
			IAsyncContextConstraint<Signaler<TParameter>?>,
			IOptionsHolderSource<Quantifier>,
			IOptionsHolderSource<SignalerOptions>,
			IOptionsHolderSource<SignalerPredicateOptions<TParameter>>
	{
		private OptionsHolder<Quantifier>? _quantifierHolder;

		private Quantifier CurrentQuantifier => _quantifierHolder is null ? quantifier : _quantifierHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<Quantifier> IOptionsHolderSource<Quantifier>.Holder
			=> _quantifierHolder ??= new OptionsHolder<Quantifier>(quantifier);

		private OptionsHolder<SignalerOptions>? _optionsHolder;

		private SignalerOptions CurrentOptions => _optionsHolder is null ? SignalerOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<SignalerOptions> IOptionsHolderSource<SignalerOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<SignalerOptions>(SignalerOptions.Default);

		private OptionsHolder<SignalerPredicateOptions<TParameter>>? _predicatesHolder;

		private SignalerPredicateOptions<TParameter> CurrentPredicates => _predicatesHolder is null ? SignalerPredicateOptions<TParameter>.Default : _predicatesHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<SignalerPredicateOptions<TParameter>> IOptionsHolderSource<SignalerPredicateOptions<TParameter>>.Holder
			=> _predicatesHolder ??= new OptionsHolder<SignalerPredicateOptions<TParameter>>(SignalerPredicateOptions<TParameter>.Default);

		private int _actualCount;
		private TimeSpan? _defaultTimeout;
		private TimeSpan? _waitedTime;

		public async ValueTask<ConstraintResult> IsMetBy(
			Signaler<TParameter>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			if (actual == null)
			{
				Actual = null;
				_defaultTimeout = null;
				Outcome = Outcome.Failure;
				return this;
			}

			SignalerPredicateOptions<TParameter> o = CurrentPredicates;
			int determinableAmount = CurrentQuantifier.DeterminableAmount;
			_defaultTimeout = GetDefaultTimeout(CurrentOptions, determinableAmount);
			TimeSpan? timeout = determinableAmount > 0 ? CurrentOptions.Timeout ?? _defaultTimeout : TimeSpan.Zero;
			long startTimestamp = context.GetTimestamp();
			// With nothing to wait for, the timeout is zero, and the Times overload rejects an amount of zero.
			Actual = await UserCode.InvokeAsync(async () => determinableAmount > 0
				? await context.WaitForSignalsAsync(actual, determinableAmount.Times(), o.Matches, timeout,
					cancellationToken)
				: await actual.WaitAsync(o.Matches, timeout, cancellationToken), "the predicate", CancellationToken.None);
			TimeSpan waited = context.GetElapsedTime(startTimestamp);
			_waitedTime = CurrentOptions.Timeout is null && _defaultTimeout is null ? null : waited;
			if (IsCanceledBeforeTheTimeout(Actual.IsSuccess, timeout, waited, context))
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			_actualCount = Actual.Parameters.Count(p => UserCode.Invoke(o.Matches, p, "the predicate"));

			Outcome = CurrentQuantifier.Check(_actualCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCallbackExpectation(stringBuilder, CurrentQuantifier, false, CurrentPredicates, CurrentOptions, _defaultTimeout);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			AppendOccurrences(stringBuilder, CurrentQuantifier, IsNegated, _actualCount);

			if (Actual?.Count > 0)
			{
				stringBuilder.Append(" in ");
				stringBuilder.Append(Formatter.Format(Actual.Parameters, FormattingOptions.MultipleLines)
					.Indent(indentation, false));
			}

			AppendWaitedTime(stringBuilder, _waitedTime, Actual?.IsSuccess);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCallbackExpectation(stringBuilder, CurrentQuantifier, true, CurrentPredicates, CurrentOptions, _defaultTimeout);
	}
}
