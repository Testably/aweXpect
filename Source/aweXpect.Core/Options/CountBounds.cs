using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     An inclusive or exclusive range of counts, without negation.
/// </summary>
/// <remarks>
///     Shared by <see cref="Quantifier" /> (how often something occurs) and <see cref="EnumerableQuantifier" /> (how many
///     items match), which only differ in their wording.
/// </remarks>
internal readonly record struct CountBounds
{
	private CountBounds(int? minimum, int? maximum, bool allowEqual)
	{
		Minimum = minimum;
		Maximum = maximum;
		AllowEqual = allowEqual;
	}

	/// <summary>
	///     The bounds of an unspecified count: at least once.
	/// </summary>
	public static CountBounds Default => new(1, null, true);

	public int? Minimum { get; }
	public int? Maximum { get; }

	/// <summary>
	///     Whether the bounds themselves are included, which is only <see langword="false" /> for <c>more than</c> and
	///     <c>fewer than</c>.
	/// </summary>
	public bool AllowEqual { get; }

	/// <summary>
	///     Whether only zero occurrences meet the bounds.
	/// </summary>
	public bool IsNever => Maximum == (AllowEqual ? 0 : 1);

	/// <summary>
	///     The amount at which <see cref="Check" /> becomes determinable without knowing whether more occurrences follow.
	/// </summary>
	public int DeterminableAmount
	{
		get
		{
			(int amount, bool isExclusive) = Maximum is null
				? (Minimum ?? 0, !AllowEqual)
				: (Maximum.Value, AllowEqual);
			return isExclusive && amount < int.MaxValue ? amount + 1 : amount;
		}
	}

	public static CountBounds AtLeast(int minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		return new CountBounds(minimum, null, true);
	}

	public static CountBounds AtMost(int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		return new CountBounds(null, maximum, true);
	}

	public static CountBounds Between(int minimum, int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<int>(minimum, maximum);
		return new CountBounds(minimum, maximum, true);
	}

	public static CountBounds Exactly(int expected)
	{
		ThrowHelper.ThrowIfCountIsNegative(expected, "expected count");
		return new CountBounds(expected, expected, true);
	}

	public static CountBounds LessThan(int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		return new CountBounds(null, maximum, false);
	}

	public static CountBounds MoreThan(int minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		return new CountBounds(minimum, null, false);
	}

	/// <summary>
	///     Returns <see langword="true" /> when the <paramref name="amount" /> meets the bounds,
	///     <see langword="false" /> when it exceeds them and <see langword="null" /> when a larger amount could still
	///     meet them, which at the <paramref name="isLast" /> amount means that it is too low.
	/// </summary>
	public bool? Check(int amount, bool isLast)
	{
		if (Maximum != null && (AllowEqual ? amount > Maximum : amount >= Maximum))
		{
			return false;
		}

		if ((isLast || Maximum == null) && !IsBelowMinimum(amount))
		{
			return true;
		}

		return null;
	}

	/// <summary>
	///     Whether the <paramref name="amount" /> is too low for the bounds.
	/// </summary>
	public bool IsBelowMinimum(int amount)
		=> Minimum != null && (AllowEqual ? amount < Minimum : amount <= Minimum);

	/// <summary>
	///     The bounds that are met exactly when these are not, or <see langword="null" /> when that is not a single
	///     range, e.g. for <c>between 1 and 3</c>.
	/// </summary>
	public CountBounds? Complement()
	{
		if (IsNever)
		{
			return new CountBounds(1, null, true);
		}

		return (Minimum, Maximum) switch
		{
			(null, { } maximum) => new CountBounds(maximum, null, !AllowEqual),
			({ } minimum, null) => new CountBounds(null, minimum, !AllowEqual),
			_ => null,
		};
	}

	/// <summary>
	///     Appends the bounds for occurrences, e.g. <c>at least twice</c>, or their complement when
	///     <paramref name="isNegated" />, e.g. <c>never</c> or <c>not exactly 3 times</c>.
	/// </summary>
	public void AppendTimes(StringBuilder stringBuilder, bool isNegated)
	{
		if (!isNegated)
		{
			AppendTimes(stringBuilder);
		}
		else if (Complement() is { } complement)
		{
			complement.AppendTimes(stringBuilder);
		}
		else
		{
			AppendTimes(stringBuilder.Append("not "));
		}
	}

	/// <summary>
	///     Appends the bounds for a number of items, e.g. <c>at least one</c> or <c>between 2 and 3</c>.
	/// </summary>
	public void AppendItems(StringBuilder stringBuilder)
	{
		switch (Minimum, Maximum)
		{
			case ({ } minimum, { } maximum) when minimum != maximum:
				stringBuilder.Append("between ").Append(minimum).Append(" and ").Append(maximum);
				return;
			case ({ } expected, not null):
				AppendItemCount(stringBuilder.Append("exactly "), expected);
				return;
			case ({ } minimum, null):
				AppendItemCount(stringBuilder.Append(AllowEqual ? "at least " : "more than "), minimum);
				return;
			default:
				AppendItemCount(stringBuilder.Append(AllowEqual ? "at most " : "fewer than "), Maximum!.Value);
				return;
		}
	}

	/// <summary>
	///     Whether the number in <see cref="AppendItems" /> is one, so that the items are named in singular.
	/// </summary>
	public bool IsSingleItem()
		=> (Minimum, Maximum) switch
		{
			({ } minimum, { } maximum) => minimum == 1 && maximum == 1,
			({ } minimum, null) => minimum == 1,
			_ => Maximum == 1,
		};

	private void AppendTimes(StringBuilder stringBuilder)
	{
		if (IsNever)
		{
			stringBuilder.Append("never");
			return;
		}

		switch (Minimum, Maximum)
		{
			case ({ } minimum, { } maximum) when minimum != maximum:
				stringBuilder.Append("between ").Append(minimum).Append(" and ").Append(maximum).Append(" times");
				return;
			case ({ } expected, not null):
				AppendOccurrences(stringBuilder.Append("exactly "), expected);
				return;
			case ({ } minimum, null):
				AppendOccurrences(stringBuilder.Append(AllowEqual ? "at least " : "more than "), minimum);
				return;
			default:
				AppendOccurrences(stringBuilder.Append(AllowEqual ? "at most " : "fewer than "), Maximum!.Value);
				return;
		}
	}

	private static void AppendItemCount(StringBuilder stringBuilder, int count)
	{
		if (count == 1)
		{
			stringBuilder.Append("one");
		}
		else
		{
			stringBuilder.Append(count);
		}
	}

	private static void AppendOccurrences(StringBuilder stringBuilder, int count)
	{
		switch (count)
		{
			case 1:
				stringBuilder.Append("once");
				break;
			case 2:
				stringBuilder.Append("twice");
				break;
			default:
				stringBuilder.Append(count).Append(" times");
				break;
		}
	}
}
