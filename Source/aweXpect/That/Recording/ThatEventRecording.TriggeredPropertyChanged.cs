using System.ComponentModel;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEventRecording
{
	/// <summary>
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChanged<TSubject>(
		this IThat<IEventRecording<TSubject>> subject)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
					filter,
					quantifier,
					options)),
			subject,
			filter,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
	/// </summary>
	/// <remarks>
	///     With <see cref="EventTriggerResult{TSubject}.Within(System.TimeSpan)" />, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching event is recorded. Without a timeout, only the
	///     events recorded so far are checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChanged<TSubject>(
		this IThat<IEventRecording<TSubject>> subject)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
					filter,
					quantifier,
					options).Invert()),
			subject,
			filter,
			quantifier,
			options);
	}
}
