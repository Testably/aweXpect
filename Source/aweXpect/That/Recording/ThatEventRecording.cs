using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;

namespace aweXpect;

/// <summary>
///     Expectations on event <see cref="IEventRecording{TSubject}" />.
/// </summary>
public static partial class ThatEventRecording
{
	private sealed class HaveTriggeredConstraint<TSubject>(
		string it,
		ExpectationGrammars grammars,
		string eventName,
		TriggerEventFilter filter)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IEventRecording<TSubject>?>,
			IOptionsHolderSource<TriggerEventFilter>,
			IOptionsHolderSource<Quantifier>,
			IOptionsHolderSource<RepeatedCheckOptions>
		where TSubject : notnull
	{
		private OptionsHolder<TriggerEventFilter>? _filterHolder;
		private OptionsHolder<Quantifier>? _quantifierHolder;
		private OptionsHolder<RepeatedCheckOptions>? _optionsHolder;

		private TriggerEventFilter CurrentFilter => _filterHolder is null ? filter : _filterHolder.Value;

		private Quantifier CurrentQuantifier
			=> _quantifierHolder is null ? Quantifier.Default : _quantifierHolder.Value;

		private RepeatedCheckOptions CurrentOptions
			=> _optionsHolder is null ? RepeatedCheckOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<TriggerEventFilter> IOptionsHolderSource<TriggerEventFilter>.Holder
			=> _filterHolder ??= new OptionsHolder<TriggerEventFilter>(filter);

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<Quantifier> IOptionsHolderSource<Quantifier>.Holder
			=> _quantifierHolder ??= new OptionsHolder<Quantifier>(Quantifier.Default);

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<RepeatedCheckOptions> IOptionsHolderSource<RepeatedCheckOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<RepeatedCheckOptions>(RepeatedCheckOptions.Default);

		private IEventRecording<TSubject>? _actual;
		private Func<IEventRecordingResult, bool>? _areFound;
		private bool _isNegated;
		private Func<object?[], bool>? _isMatch;
		private IEventRecordingResult? _result;
		private bool _stoppedEarly;
		private TimeSpan? _waitedTime;

		public async ValueTask<ConstraintResult> IsMetBy(IEventRecording<TSubject>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_isMatch = CurrentFilter.IsMatch;
			long? startTimestamp = CurrentOptions.IsRepeated ? context.GetTimestamp() : null;
			_result = await actual.StopWhen(_areFound ??= AreFound, CurrentOptions.Timeout, context, cancellationToken);
			int eventCount = _result.GetEventCount(eventName, _isMatch);
			if (startTimestamp is not null)
			{
				_waitedTime = context.GetElapsedTime(startTimestamp.Value);
				_stoppedEarly = CurrentQuantifier.Check(eventCount, false) != null;
				if (!_stoppedEarly && cancellationToken.IsCancellationRequested &&
				    !context.Cancellation.HasWaitElapsed(CurrentOptions.Timeout, _waitedTime.Value))
				{
					// A cancellation can end the wait before the timeout, so the events recorded until then decide nothing.
					Outcome = Outcome.Undecided;
					return this;
				}
			}

			Outcome = CurrentQuantifier.Check(eventCount, true, _isNegated) ?? _isNegated ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (CurrentQuantifier.IsNever(_isNegated))
			{
				stringBuilder.Append("has never recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(CurrentFilter).Append(CurrentOptions);
			}
			else
			{
				stringBuilder.Append("has recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(CurrentFilter).Append(' ').Append(CurrentQuantifier.ToString(_isNegated)).Append(CurrentOptions);
			}
		}

		private bool AreFound(IEventRecordingResult result)
			=> CurrentQuantifier.Check(result.GetEventCount(eventName, _isMatch!), false) != null;

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it);
				return;
			}

			if (Outcome == Outcome.Undecided)
			{
				AppendCanceledResult(stringBuilder, it);
				return;
			}

			stringBuilder.Append(it).Append(" was ");
			ThatSignaler.AppendOccurrences(stringBuilder, CurrentQuantifier, _isNegated,
				_result?.GetEventCount(eventName, _isMatch) ?? 0);
			if (_result?.GetEventCount(eventName) > 0)
			{
				stringBuilder.Append(" in ").Append(_result.ToString(eventName));
			}

			if (_waitedTime is not null)
			{
				stringBuilder.Append(_stoppedEarly ? " after " : " within ");
				Formatter.Format(stringBuilder, _waitedTime.Value);
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEventRecording<TSubject>));
		}

		protected override void NegateInPlace()
		{
			_isNegated = !_isNegated;
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
		}
	}
}
