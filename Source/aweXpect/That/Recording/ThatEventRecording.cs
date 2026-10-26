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
		OptionsHolder<TriggerEventFilter> filter,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<RepeatedCheckOptions> options)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IEventRecording<TSubject>>
		where TSubject : notnull
	{
		private IEventRecording<TSubject>? _actual;
		private Func<IEventRecordingResult, bool>? _areFound;
		private bool _isNegated;
		private Func<object?[], bool>? _isMatch;
		private IEventRecordingResult? _result;
		private bool _stoppedEarly;
		private TimeSpan? _waitedTime;

		public async ValueTask<ConstraintResult> IsMetBy(IEventRecording<TSubject> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_isMatch = filter.Value.IsMatch;
			long? startTimestamp = options.Value.IsRepeated ? context.GetTimestamp() : null;
			_result = await actual.StopWhen(_areFound ??= AreFound, options.Value.Timeout, context, cancellationToken);
			int eventCount = _result.GetEventCount(eventName, _isMatch);
			if (startTimestamp is not null)
			{
				_waitedTime = context.GetElapsedTime(startTimestamp.Value);
				_stoppedEarly = quantifier.Value.Check(eventCount, false) != null;
				if (!_stoppedEarly && cancellationToken.IsCancellationRequested &&
				    !context.Cancellation.HasWaitElapsed(options.Value.Timeout, _waitedTime.Value))
				{
					// A cancellation can end the wait before the timeout, so the events recorded until then decide nothing.
					Outcome = Outcome.Undecided;
					return this;
				}
			}

			Outcome = quantifier.Value.Check(eventCount, true, _isNegated) ?? _isNegated ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (quantifier.Value.IsNever(_isNegated))
			{
				stringBuilder.Append("has never recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(filter).Append(options);
			}
			else
			{
				stringBuilder.Append("has recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(filter).Append(' ').Append(quantifier.Value.ToString(_isNegated)).Append(options.Value);
			}
		}

		private bool AreFound(IEventRecordingResult result)
			=> quantifier.Value.Check(result.GetEventCount(eventName, _isMatch!), false) != null;

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
			ThatSignaler.AppendOccurrences(stringBuilder, quantifier.Value, _isNegated,
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
