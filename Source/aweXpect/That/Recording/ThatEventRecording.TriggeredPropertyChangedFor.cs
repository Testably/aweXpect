using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
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
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the property given by the <paramref name="propertyExpression" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChangedFor<TSubject, TProperty>(
		this IThat<IEventRecording<TSubject>> subject,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
		=> TriggeredPropertyChangedFor(subject, GetPropertyName(propertyExpression));

	/// <summary>
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> subject,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			MatchesPropertyName(propertyName),
			$" for property {propertyName}");
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
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the property given by the <paramref name="propertyExpression" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject, TProperty>(
		this IThat<IEventRecording<TSubject>> subject,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
		=> DidNotTriggerPropertyChangedFor(subject, GetPropertyName(propertyExpression));

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> subject,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			MatchesPropertyName(propertyName),
			$" for property {propertyName}");
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

	/// <summary>
	///     Creates the predicate that matches the recorded <see cref="PropertyChangedEventArgs.PropertyName" />
	///     against the expected <paramref name="propertyName" />.
	/// </summary>
	/// <remarks>
	///     The <see cref="INotifyPropertyChanged" /> contract declares a recorded <see langword="null" /> or empty
	///     property name as "all properties changed", so it has to satisfy the expectation for any property. Expecting
	///     such a name itself matches only this notification, but makes no difference between its two spellings, which
	///     the contract does not distinguish either.
	/// </remarks>
	private static Func<object?[], bool> MatchesPropertyName(string? propertyName)
	{
		if (string.IsNullOrEmpty(propertyName))
		{
			return o => o.Length > 1 && o[1] is PropertyChangedEventArgs m && string.IsNullOrEmpty(m.PropertyName);
		}

		return o => o.Length > 1 && o[1] is PropertyChangedEventArgs m &&
		            (m.PropertyName == propertyName || string.IsNullOrEmpty(m.PropertyName));
	}

	/// <summary>
	///     Extracts the property name from the <paramref name="propertyExpression" />.
	/// </summary>
	/// <remarks>
	///     Rejecting anything but a property access keeps an unusable expression from silently becoming the
	///     <see langword="null" /> property name, which would match events raised without a property name.
	/// </remarks>
	private static string GetPropertyName<TSubject, TProperty>(
		Expression<Func<TSubject, TProperty>> propertyExpression)
	{
		MemberInfo? memberInfo =
			(((propertyExpression.Body as UnaryExpression)?.Operand ?? propertyExpression.Body) as MemberExpression)
			?.Member;
		if (memberInfo is not PropertyInfo propertyInfo)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentException(
				$"The 'propertyExpression' must refer to a property, but was '{propertyExpression.Body}'.",
				nameof(propertyExpression));
		}

		return propertyInfo.Name;
	}
}
