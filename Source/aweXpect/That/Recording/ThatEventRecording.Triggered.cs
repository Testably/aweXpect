using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;
using aweXpect.Results;

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
		this IThat<IEventRecording<TSubject>> source,
		string eventName)
		where TSubject : notnull
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		return new EventTriggerResult<TSubject>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveTriggeredConstraint<TSubject>(it, grammars, eventName, filter, quantifier, options)),
			source,
			filter,
			quantifier,
			options);
	}
}
