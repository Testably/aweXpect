using System.ComponentModel;
using aweXpect.Core;
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
		this IThat<IEventRecording<TSubject>> source)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		return new EventTriggerResult<TSubject>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
					filter,
					quantifier,
					options)),
			source,
			filter,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChanged<TSubject>(
		this IThat<IEventRecording<TSubject>> source)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		quantifier.Exactly(0);
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		return new EventTriggerResult<TSubject>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
					filter,
					quantifier,
					options)),
			source,
			filter,
			quantifier,
			options);
	}
}
