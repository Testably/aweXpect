using System;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Results;

/// <summary>
///     Result for the underlying value of an enum that continues on the <see cref="IThat{TItem}" /> subject.
/// </summary>
public class EnumValueResult<TItem> : EnumValueResult<TItem, TItem>
{
	internal EnumValueResult(IThat<TItem> subject, Func<TItem, decimal?> mapper, string propertyExpression)
		: base(subject, mapper, propertyExpression)
	{
	}
}

/// <summary>
///     Result for the underlying value of an enum of a <typeparamref name="TValue" /> which continues on the
///     <see cref="IThat{TValue}" /> subject with an underlying value of type <typeparamref name="TType" />.
/// </summary>
/// <remarks>
///     <typeparamref name="TType" /> differs from <typeparamref name="TValue" /> for a nullable subject, whose value is
///     no longer <see langword="null" /> once a comparison succeeded.
///     <para />
///     Every comparison comes in a <see langword="long" /> and a <see langword="ulong" /> flavour, because neither
///     type alone names every enum value: only a <see langword="ulong" /> reaches a member above
///     <see cref="long.MaxValue" />, and only a <see langword="long" /> a negative one. Both are legal attribute
///     arguments, so a comparison can be driven from an <c>[InlineData]</c>; a bare <see langword="null" /> binds to
///     the <see langword="long" /> flavour.
/// </remarks>
public class EnumValueResult<TValue, TType>
{
	private readonly Func<TValue, decimal?> _mapper;
	private readonly string _propertyExpression;
	private readonly IThat<TValue> _subject;

	internal EnumValueResult(IThat<TValue> subject, Func<TValue, decimal?> mapper, string propertyExpression)
	{
		_subject = subject;
		_mapper = mapper;
		_propertyExpression = propertyExpression;
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> EqualTo(long? expected)
		=> AddEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> EqualTo(ulong? expected)
		=> AddEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotEqualTo(long? unexpected)
		=> AddNotEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotEqualTo(ulong? unexpected)
		=> AddNotEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> GreaterThan(long? expected)
		=> AddGreaterThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> GreaterThan(ulong? expected)
		=> AddGreaterThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotGreaterThan(long? unexpected)
		=> AddNotGreaterThan(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not greater than the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotGreaterThan(ulong? unexpected)
		=> AddNotGreaterThan(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> GreaterThanOrEqualTo(long? expected)
		=> AddGreaterThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> GreaterThanOrEqualTo(ulong? expected)
		=> AddGreaterThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotGreaterThanOrEqualTo(long? unexpected)
		=> AddNotGreaterThanOrEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not greater than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotGreaterThanOrEqualTo(ulong? unexpected)
		=> AddNotGreaterThanOrEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> LessThan(long? expected)
		=> AddLessThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> LessThan(ulong? expected)
		=> AddLessThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotLessThan(long? unexpected)
		=> AddNotLessThan(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not less than the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotLessThan(ulong? unexpected)
		=> AddNotLessThan(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> LessThanOrEqualTo(long? expected)
		=> AddLessThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> LessThanOrEqualTo(ulong? expected)
		=> AddLessThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotLessThanOrEqualTo(long? unexpected)
		=> AddNotLessThanOrEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not less than or equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> NotLessThanOrEqualTo(ulong? unexpected)
		=> AddNotLessThanOrEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TType, IThat<TValue>>, long?> Between(long? minimum)
		=> new(maximum => AddBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TType, IThat<TValue>>, ulong?> Between(ulong? minimum)
		=> new(maximum => AddBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	/// <summary>
	///     …is not between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TType, IThat<TValue>>, long?> NotBetween(long? minimum)
		=> new(maximum => AddNotBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	/// <summary>
	///     …is not between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TType, IThat<TValue>>, ulong?> NotBetween(ulong? minimum)
		=> new(maximum => AddNotBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	private AndOrResult<TType, IThat<TValue>> AddEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual?.Equals(expected) == true, $"equal to {formattedExpected}");

	private AndOrResult<TType, IThat<TValue>> AddNotEqualTo(decimal? unexpected, string formattedUnexpected)
		=> Add(actual => actual?.Equals(unexpected) != true, $"equal to {formattedUnexpected}", true);

	private AndOrResult<TType, IThat<TValue>> AddGreaterThan(decimal? expected, string formattedExpected)
		=> Add(actual => actual > expected, $"greater than {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TType, IThat<TValue>> AddNotGreaterThan(decimal? expected, string formattedExpected)
		=> Add(actual => !(actual > expected), $"greater than {formattedExpected}", true,
			expected is null);

	private AndOrResult<TType, IThat<TValue>> AddGreaterThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual >= expected, $"greater than or equal to {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TType, IThat<TValue>> AddNotGreaterThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => !(actual >= expected), $"greater than or equal to {formattedExpected}", true,
			expected is null);

	private AndOrResult<TType, IThat<TValue>> AddLessThan(decimal? expected, string formattedExpected)
		=> Add(actual => actual < expected, $"less than {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TType, IThat<TValue>> AddNotLessThan(decimal? expected, string formattedExpected)
		=> Add(actual => !(actual < expected), $"less than {formattedExpected}", true,
			expected is null);

	private AndOrResult<TType, IThat<TValue>> AddLessThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual <= expected, $"less than or equal to {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TType, IThat<TValue>> AddNotLessThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => !(actual <= expected), $"less than or equal to {formattedExpected}", true,
			expected is null);

	private AndOrResult<TType, IThat<TValue>> AddBetween(decimal? minimum, decimal? maximum,
		string formattedMinimum, string formattedMaximum)
	{
		ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
		return Add(actual => actual >= minimum && actual <= maximum,
			$"between {formattedMinimum} and {formattedMaximum}",
			isOrderedAgainstNull: minimum is null || maximum is null);
	}

	private AndOrResult<TType, IThat<TValue>> AddNotBetween(decimal? minimum, decimal? maximum,
		string formattedMinimum, string formattedMaximum)
	{
		ThrowHelper.ThrowIfMaximumIsBelowMinimum(minimum, maximum);
		return Add(actual => !(actual >= minimum && actual <= maximum),
			$"between {formattedMinimum} and {formattedMaximum}", true,
			minimum is null || maximum is null);
	}

	private AndOrResult<TType, IThat<TValue>> Add(
		Func<decimal?, bool> condition,
		string expectation,
		bool isNegative = false,
		bool isOrderedAgainstNull = false)
		=> new(_subject.Get().ExpectationBuilder
				.AddConstraint((Mapper: _mapper, PropertyExpression: _propertyExpression, Condition: condition,
						Expectation: expectation, IsNegative: isNegative, IsOrderedAgainstNull: isOrderedAgainstNull),
					static (state, it, grammars) =>
						new ValueConstraint(it, grammars, state.Mapper, state.PropertyExpression, state.Condition,
							state.Expectation, state.IsNegative, state.IsOrderedAgainstNull)),
			_subject);

	private sealed class ValueConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TValue, decimal?> mapper,
		string propertyExpression,
		Func<decimal?, bool> condition,
		string expectation,
		bool isNegative,
		bool isOrderedAgainstNull)
		: ConstraintResult.WithNotNullValue<TValue>(it, grammars),
			IValueConstraint<TValue>
	{
		private decimal? _value;

		public ConstraintResult IsMetBy(TValue actual)
		{
			Actual = actual;
			_value = mapper(actual);
			Outcome = condition(_value) ? Outcome.Success : Outcome.Failure;
			if (isOrderedAgainstNull)
			{
				Outcome = Outcome.FailureBothWays;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, isNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested) && !Grammars.HasFlag(ExpectationGrammars.Active))
			{
				// The "whose" clause made the property the subject.
				stringBuilder.Append(propertyExpression).Append(" was ");
			}
			else
			{
				stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ');
			}

			stringBuilder.Append(_value?.ToString(CultureInfo.InvariantCulture) ?? ValueFormatter.NullString);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, !isNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void Append(StringBuilder stringBuilder, bool isNegated)
		{
			string negation = isNegated ? "not " : "";
			if (Grammars.HasFlag(ExpectationGrammars.Active))
			{
				stringBuilder.Append("with ").Append(propertyExpression).Append(' ').Append(negation);
			}
			else if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("whose ").Append(propertyExpression).Append(" is ").Append(negation);
			}
			else
			{
				stringBuilder.Append(isNegated
						? Grammars.Verb("does not have ", "do not have ")
						: Grammars.Verb("has ", "have "))
					.Append(propertyExpression).Append(' ');
			}

			stringBuilder.Append(expectation);
		}
	}
}
