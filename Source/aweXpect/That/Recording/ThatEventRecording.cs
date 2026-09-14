using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
		TriggerEventFilter filter,
		Quantifier quantifier,
		RepeatedCheckOptions options)
		: ConstraintResult(grammars),
			IAsyncConstraint<IEventRecording<TSubject>>
		where TSubject : notnull
	{
		private IEventRecording<TSubject>? _actual;
		private IEventRecordingResult? _result;

		public async Task<ConstraintResult> IsMetBy(IEventRecording<TSubject> actual,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_result = await actual.StopWhen(result =>
				quantifier.Check(result.GetEventCount(eventName, filter.IsMatch), true) == true, options.Timeout);
			int eventCount = _result.GetEventCount(eventName, filter.IsMatch);
			Outcome = quantifier.Check(eventCount, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (quantifier.IsNever)
			{
				stringBuilder.Append("has never recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(filter);
			}
			else
			{
				stringBuilder.Append("has recorded the ").Append(eventName).Append(" event");
				if (_actual != null)
				{
					stringBuilder.Append(" on ").Append(_actual);
				}

				stringBuilder.Append(filter).Append(' ').Append(quantifier).Append(options);
			}
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it);
				return;
			}

			int? eventCount = _result?.GetEventCount(eventName, filter.IsMatch);
			stringBuilder.Append(it).Append(" was ");
			if (eventCount == 0)
			{
				stringBuilder.Append("never recorded ");
			}
			else if (eventCount == 1)
			{
				stringBuilder.Append("recorded once ");
			}
			else if (eventCount == 2)
			{
				stringBuilder.Append("recorded twice ");
			}
			else
			{
				stringBuilder.Append("recorded ").Append(eventCount).Append(" times ");
			}

			stringBuilder.Append("in ");
			stringBuilder.Append(_result?.ToString(eventName));
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IEventRecording<TSubject>));
		}

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome
		{
			get => _actual is null ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public override ConstraintResult Negate()
		{
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
