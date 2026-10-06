using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Recording;
using aweXpect.Results;
using RepeatedCheckOptions = aweXpect.Options.RepeatedCheckOptions;
using TriggerEventFilter = aweXpect.Options.TriggerEventFilter;

namespace aweXpect;

public static partial class ThatEventRecording
{
	/// <summary>
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the property given by the <paramref name="propertyExpression" />.
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChangedFor<TSubject, TProperty>(
		this IThat<IEventRecording<TSubject>> subject,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
		=> subject.TriggeredPropertyChangedFor(GetPropertyName(propertyExpression));

	/// <summary>
	///     Verifies that the subject has triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />.
	/// </summary>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> TriggeredPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> subject,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Options.Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			MatchesPropertyName(propertyName),
			DescribePropertyName(propertyName));
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint((Filter: filter, Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
						state.Filter,
						state.Quantifier,
						state.Options)),
			subject,
			filter,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the property given by the <paramref name="propertyExpression" />.
	/// </summary>
	/// <remarks>
	///     With <see cref="EventTriggerResult{TSubject}.Within(System.TimeSpan)" />, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching event is recorded. Without a timeout, only the
	///     events recorded so far are checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject, TProperty>(
		this IThat<IEventRecording<TSubject>> subject,
		Expression<Func<TSubject, TProperty>> propertyExpression)
		where TSubject : INotifyPropertyChanged
		=> subject.DidNotTriggerPropertyChangedFor(GetPropertyName(propertyExpression));

	/// <summary>
	///     Verifies that the subject has not triggered the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
	///     for the given <paramref name="propertyName" />.
	/// </summary>
	/// <remarks>
	///     With <see cref="EventTriggerResult{TSubject}.Within(System.TimeSpan)" />, the expectation waits for the full
	///     timeout before it can succeed, and fails as soon as a matching event is recorded. Without a timeout, only the
	///     events recorded so far are checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static EventTriggerResult<TSubject> DidNotTriggerPropertyChangedFor<TSubject>(
		this IThat<IEventRecording<TSubject>> subject,
		string? propertyName)
		where TSubject : INotifyPropertyChanged
	{
		Options.Quantifier quantifier = new();
		TriggerEventFilter filter = new();
		RepeatedCheckOptions options = new();
		filter.AddPredicate(
			MatchesPropertyName(propertyName),
			DescribePropertyName(propertyName));
		return new EventTriggerResult<TSubject>(
			subject.Get().ExpectationBuilder.AddConstraint((Filter: filter, Quantifier: quantifier, Options: options),
				static (state, it, grammars)
					=> new HaveTriggeredConstraint<TSubject>(it, grammars, nameof(INotifyPropertyChanged.PropertyChanged),
						state.Filter,
						state.Quantifier,
						state.Options).Invert()),
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

	/// <remarks>
	///     A <see langword="null" /> or empty property name is the "all properties changed" notification of the
	///     <see cref="INotifyPropertyChanged" /> contract.
	/// </remarks>
	private static string DescribePropertyName(string? propertyName)
		=> string.IsNullOrEmpty(propertyName) ? " for all properties" : $" for property {propertyName}";

	/// <summary>
	///     Extracts the property name from the <paramref name="propertyExpression" />.
	/// </summary>
	/// <remarks>
	///     Rejecting anything but a property access keeps an unusable expression from silently becoming the
	///     <see langword="null" /> property name, which would match events raised without a property name.
	///     <para />
	///     The property has to be accessed on the subject itself, because the subject does not report the changes of
	///     a property of another object under the name of that property.
	/// </remarks>
	private static string GetPropertyName<TSubject, TProperty>(
		Expression<Func<TSubject, TProperty>> propertyExpression)
	{
		propertyExpression.ThrowIfNull();
		if (WithoutConversions(propertyExpression.Body, true) is not MemberExpression
		    {
			    Member: PropertyInfo propertyInfo,
		    } memberExpression ||
		    WithoutConversions(memberExpression.Expression, false) != propertyExpression.Parameters[0])
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The 'propertyExpression' must refer to a property, but it was {propertyExpression.Body}.",
				nameof(propertyExpression)));
		}

		return propertyInfo.Name;
	}

	/// <remarks>
	///     A conversion of the property value still names the property, whereas a conversion of the subject with a
	///     conversion operator results in another object.
	/// </remarks>
	private static Expression? WithoutConversions(Expression? expression, bool includingOperators)
	{
		while (expression is UnaryExpression
		       {
			       NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.TypeAs,
		       } conversion &&
		       (includingOperators || conversion.Method is null))
		{
			expression = conversion.Operand;
		}

		return expression;
	}
}
