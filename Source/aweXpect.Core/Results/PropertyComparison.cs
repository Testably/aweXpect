using System;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Results;

/// <summary>
///     The comparison of a <typeparamref name="TProperty" /> property that an operator like <c>GreaterThan(…)</c> passes
///     to the <c>Compare</c> method of a property result, e.g. of an <see cref="IIntPropertyResult{TResult}" />.
/// </summary>
/// <remarks>
///     It is immutable, so a result can store it and evaluate it later, also several times. The expectation text is only
///     formatted when a message needs it, as most expectations are met.
/// </remarks>
public sealed class PropertyComparison<TProperty>
	where TProperty : struct
{
	private readonly Func<TProperty?, TProperty?, TProperty?, bool> _condition;
	private readonly TProperty? _expected;
	private readonly string _expectationPrefix;
	private readonly bool _hasMaximum;
	private readonly bool _isOrderedAgainstNull;
	private readonly TProperty? _maximum;

	internal PropertyComparison(
		TProperty? expected,
		Func<TProperty?, TProperty?, TProperty?, bool> condition,
		string expectationPrefix,
		bool isNegated = false,
		bool isOrderedAgainstNull = false,
		bool hasMaximum = false,
		TProperty? maximum = null)
	{
		_expected = expected;
		_condition = condition;
		_expectationPrefix = expectationPrefix;
		IsNegated = isNegated;
		_isOrderedAgainstNull = isOrderedAgainstNull;
		_hasMaximum = hasMaximum;
		_maximum = maximum;
	}

	/// <summary>
	///     Flag indicating that the operator is a negated one, e.g. <c>NotGreaterThan(…)</c>.
	/// </summary>
	/// <remarks>
	///     <see cref="Evaluate(TProperty?)" /> already includes the negation, but
	///     <see cref="AppendExpectation(StringBuilder)" /> does not, so that the "not" can be placed where the sentence
	///     needs it, e.g. "has major version not greater than 2" or "does not have major version greater than 2".
	/// </remarks>
	public bool IsNegated { get; }

	/// <summary>
	///     Appends the comparison without its negation, e.g. "greater than 2" or "between 1 and 3", to the
	///     <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder)
	{
		stringBuilder.Append(_expectationPrefix);
		Formatter.Format(stringBuilder, _expected);
		if (_hasMaximum)
		{
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, _maximum);
		}
	}

	/// <summary>
	///     Evaluates the comparison against the <paramref name="actual" /> value of the property.
	/// </summary>
	/// <remarks>
	///     An ordering against a <see langword="null" /> bound cannot be answered, so it returns
	///     <see cref="Outcome.FailureBothWays" />, which fails the expectation and its negation.
	/// </remarks>
	public Outcome Evaluate(TProperty? actual)
	{
		if (_isOrderedAgainstNull)
		{
			return Outcome.FailureBothWays;
		}

		return _condition(actual, _expected, _maximum) ? Outcome.Success : Outcome.Failure;
	}
}
