using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using aweXpect.Core;
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
		this IThat<IEventRecording<TSubject>> source,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
	{
		MemberInfo? memberInfo =
			(((propertyExpression.Body as UnaryExpression)?.Operand ?? propertyExpression.Body) as MemberExpression)
			?.Member;
		string? propertyName = (memberInfo as PropertyInfo)?.Name;
		return TriggeredPropertyChangedFor(source, propertyName);
	}

	/// <summary>
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> source,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			o => o.Length > 1 && o[1] is PropertyChangedEventArgs m && m.PropertyName == propertyName,
			$" for property {propertyName}");
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
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the property given by the <paramref name="propertyExpression" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject, TProperty>(
		this IThat<IEventRecording<TSubject>> source,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
	{
		MemberInfo? memberInfo =
			(((propertyExpression.Body as UnaryExpression)?.Operand ?? propertyExpression.Body) as MemberExpression)
			?.Member;
		string? propertyName = (memberInfo as PropertyInfo)?.Name;
		return DidNotTriggerPropertyChangedFor(source, propertyName);
	}

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> source,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Quantifier quantifier = new();
		quantifier.Exactly(0);
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			o => o.Length > 1 && o[1] is PropertyChangedEventArgs m && m.PropertyName == propertyName,
			$" for property {propertyName}");
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
