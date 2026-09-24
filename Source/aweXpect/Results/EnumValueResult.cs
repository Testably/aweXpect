using System;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;

namespace aweXpect.Results;

/// <summary>
///     Result for the underlying value of an enum that continues on the <see cref="IThat{TItem}" /> subject.
/// </summary>
/// <remarks>
///     Every comparison comes in a <see langword="long" /> and a <see langword="ulong" /> flavour, because neither
///     type alone names every enum value: only a <see langword="ulong" /> reaches a member above
///     <see cref="long.MaxValue" />, and only a <see langword="long" /> a negative one. Both are legal attribute
///     arguments, so a comparison can be driven from an <c>[InlineData]</c>; a bare <see langword="null" /> binds to
///     the <see langword="long" /> flavour.
/// </remarks>
public class EnumValueResult<TItem>
{
	private readonly Func<TItem, decimal?> _mapper;
	private readonly string _propertyExpression;
	private readonly IThat<TItem> _subject;

	internal EnumValueResult(IThat<TItem> subject, Func<TItem, decimal?> mapper, string propertyExpression)
	{
		_subject = subject;
		_mapper = mapper;
		_propertyExpression = propertyExpression;
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> EqualTo(long? expected)
		=> AddEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> EqualTo(ulong? expected)
		=> AddEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> NotEqualTo(long? unexpected)
		=> AddNotEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> NotEqualTo(ulong? unexpected)
		=> AddNotEqualTo(unexpected, Formatter.Format(unexpected));

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> GreaterThan(long? expected)
		=> AddGreaterThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is greater than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> GreaterThan(ulong? expected)
		=> AddGreaterThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(long? expected)
		=> AddGreaterThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is greater than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> GreaterThanOrEqualTo(ulong? expected)
		=> AddGreaterThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> LessThan(long? expected)
		=> AddLessThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> LessThan(ulong? expected)
		=> AddLessThan(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(long? expected)
		=> AddLessThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is less than or equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<TItem, IThat<TItem>> LessThanOrEqualTo(ulong? expected)
		=> AddLessThanOrEqualTo(expected, Formatter.Format(expected));

	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TItem, IThat<TItem>>, long?> Between(long? minimum)
		=> new(maximum => AddBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	/// <summary>
	///     …is between the <paramref name="minimum" />…
	/// </summary>
	public BetweenResult<AndOrResult<TItem, IThat<TItem>>, ulong?> Between(ulong? minimum)
		=> new(maximum => AddBetween(minimum, maximum, Formatter.Format(minimum), Formatter.Format(maximum)));

	private AndOrResult<TItem, IThat<TItem>> AddEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual?.Equals(expected) == true, $"equal to {formattedExpected}");

	private AndOrResult<TItem, IThat<TItem>> AddNotEqualTo(decimal? unexpected, string formattedUnexpected)
		=> Add(actual => actual?.Equals(unexpected) != true, $"not equal to {formattedUnexpected}",
			$"equal to {formattedUnexpected}");

	private AndOrResult<TItem, IThat<TItem>> AddGreaterThan(decimal? expected, string formattedExpected)
		=> Add(actual => actual > expected, $"greater than {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TItem, IThat<TItem>> AddGreaterThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual >= expected, $"greater than or equal to {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TItem, IThat<TItem>> AddLessThan(decimal? expected, string formattedExpected)
		=> Add(actual => actual < expected, $"less than {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TItem, IThat<TItem>> AddLessThanOrEqualTo(decimal? expected, string formattedExpected)
		=> Add(actual => actual <= expected, $"less than or equal to {formattedExpected}",
			isOrderedAgainstNull: expected is null);

	private AndOrResult<TItem, IThat<TItem>> AddBetween(decimal? minimum, decimal? maximum,
		string formattedMinimum, string formattedMaximum)
		=> Add(actual => actual >= minimum && actual <= maximum,
			$"between {formattedMinimum} and {formattedMaximum}",
			isOrderedAgainstNull: minimum is null || maximum is null);

	private AndOrResult<TItem, IThat<TItem>> Add(
		Func<decimal?, bool> condition,
		string expectation,
		string? negatedExpectation = null,
		bool isOrderedAgainstNull = false)
		=> new(_subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new ValueConstraint(it, grammars, _mapper, _propertyExpression, condition, expectation,
						negatedExpectation, isOrderedAgainstNull)),
			_subject);

	private sealed class ValueConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<TItem, decimal?> mapper,
		string propertyExpression,
		Func<decimal?, bool> condition,
		string expectation,
		string? negatedExpectation,
		bool isOrderedAgainstNull)
		: ConstraintResult.WithNotNullValue<TItem>(it, grammars),
			IValueConstraint<TItem>
	{
		private decimal? _value;

		/// <inheritdoc />
		public override Outcome Outcome
		{
			get => isOrderedAgainstNull ? Outcome.Failure : base.Outcome;
			protected set => base.Outcome = value;
		}

		public ConstraintResult IsMetBy(TItem actual)
		{
			Actual = actual;
			_value = mapper(actual);
			Outcome = condition(_value) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, false, expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had ").Append(propertyExpression).Append(' ')
				.Append(_value?.ToString(CultureInfo.InvariantCulture) ?? ValueFormatter.NullString);

		// A comparison that is itself a negation (`not equal to`) is negated by its positive counterpart instead of by
		// a second `not`.
		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> Append(stringBuilder, negatedExpectation is null, negatedExpectation ?? expectation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void Append(StringBuilder stringBuilder, bool isNegated, string comparison)
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

			stringBuilder.Append(comparison);
		}
	}
}
