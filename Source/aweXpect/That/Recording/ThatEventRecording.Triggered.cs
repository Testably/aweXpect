using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Recording;
using aweXpect.Results;
using RepeatedCheckOptions = aweXpect.Options.RepeatedCheckOptions;
using TriggerEventFilter = aweXpect.Options.TriggerEventFilter;

namespace aweXpect;

public static partial class ThatEventRecording
{
	/// <summary>
	///     Verifies that the subject has triggered the expected <paramref name="eventName" />.
	/// </summary>
	/// <remarks>
	///     This will stop the recording on the <see cref="IEventRecording{TSubject}" /> subject.
	/// </remarks>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> Triggered<TSubject>(
		this IThat<IEventRecording<TSubject>?> subject,
		string eventName)
		where TSubject : notnull
	{
		eventName.ThrowIfNull();
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint(eventName,
				static (eventName, it, grammars)
					=> new HaveTriggeredConstraint<TSubject>(it, grammars, eventName, TriggerEventFilter.Default),
				out HaveTriggeredConstraint<TSubject> constraint),
			subject,
			constraint,
			constraint,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject has not triggered the expected <paramref name="eventName" />.
	/// </summary>
	/// <remarks>
	///     This will stop the recording on the <see cref="IEventRecording{TSubject}" /> subject.
	///     <para />
	///     With <see cref="EventTriggerResult{TSubject}.Within(System.TimeSpan)" />, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching event is recorded. Without a timeout, only the
	///     events recorded so far are checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTrigger<TSubject>(
		this IThat<IEventRecording<TSubject>?> subject,
		string eventName)
		where TSubject : notnull
	{
		eventName.ThrowIfNull();
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint(eventName,
				static (eventName, it, grammars)
					=> new HaveTriggeredConstraint<TSubject>(it, grammars, eventName, TriggerEventFilter.Default)
						.Invert(),
				out HaveTriggeredConstraint<TSubject> constraint),
			subject,
			constraint,
			constraint,
			constraint);
	}
}
