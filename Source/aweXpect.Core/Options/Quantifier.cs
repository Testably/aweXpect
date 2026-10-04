using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Quantifies an occurrence.
/// </summary>
public class Quantifier
{
	private bool _allowEqual = true;
	private bool _isNegated;
	private int? _maximum;
	private int? _minimum = 1;
	private string? _specifiedBy;

	/// <summary>
	///     Flag indicating if the <see cref="Quantifier" /> is negated.
	/// </summary>
	public bool IsNegated => _isNegated;

	/// <summary>
	///     Flag indicating if the <see cref="Quantifier" /> is equivalent to never.
	/// </summary>
	public bool IsNever => _isNegated switch
	{
		false => _allowEqual && _maximum == 0,
		true => _allowEqual && _minimum == 1 && _maximum == null,
	};

	/// <summary>
	///     The amount at which <see cref="Check(int, bool)" /> becomes determinable without knowing whether more
	///     occurrences follow.
	/// </summary>
	/// <remarks>
	///     Callers that observe occurrences over time can stop as soon as this many occurred; below it they have to
	///     wait for their timeout to expire, because only then is the amount final.
	/// </remarks>
	public int DeterminableAmount
	{
		get
		{
			(int amount, bool isExclusive) = _maximum is null
				? (_minimum ?? 0, !_allowEqual)
				: (_maximum.Value, _allowEqual);
			return isExclusive && amount < int.MaxValue ? amount + 1 : amount;
		}
	}

	/// <summary>
	///     A quantifier for exactly zero items.
	/// </summary>
	public static Quantifier Never()
	{
		Quantifier quantifier = new();
		quantifier.Exactly(0);
		return quantifier;
	}

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void AtLeast(int minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		Set(minimum, null, true, nameof(AtLeast));
	}

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void AtMost(int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		Set(null, maximum, true, nameof(AtMost));
	}

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" /> and <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void Between(int minimum, int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<int>(minimum, maximum);
		Set(minimum, maximum, true, nameof(Between));
	}

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void LessThan(int maximum)
	{
		ThrowHelper.ThrowIfCountIsNegative(maximum);
		Set(null, maximum, false, nameof(LessThan));
	}

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void MoreThan(int minimum)
	{
		ThrowHelper.ThrowIfCountIsNegative(minimum);
		Set(minimum, null, false, nameof(MoreThan));
	}

	/// <summary>
	///     Verifies the amount against the conditions.
	/// </summary>
	/// <remarks>
	///     Returns <see langword="true" /> when the condition is satisfied,
	///     <see langword="false" /> when the condition is not satisfied
	///     and <see langword="null" /> when the condition could still be satisfied
	///     with a larger <paramref name="amount" />.
	/// </remarks>
	public bool? Check(int amount, bool isLast)
	{
		if (_maximum != null && (_allowEqual ? amount > _maximum : amount >= _maximum))
		{
			return _isNegated;
		}

		if ((isLast || _maximum == null) &&
		    (_minimum == null || (_allowEqual ? amount >= _minimum : amount > _minimum)))
		{
			return !_isNegated;
		}

		return null;
	}

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	/// <exception cref="InvalidOperationException">The quantifier is already specified.</exception>
	public void Exactly(int expected)
		=> Exactly(expected, nameof(Exactly));

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times, as specified by the <paramref name="option" />.
	/// </summary>
	internal void Exactly(int expected, string option)
	{
		ThrowHelper.ThrowIfCountIsNegative(expected, "expected count");
		Set(expected, expected, true, option);
	}

	/// <summary>
	///     Rejects a second quantifier, because it would silently replace the bounds of the first one.
	/// </summary>
	private void Set(int? minimum, int? maximum, bool allowEqual, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_specifiedBy, option);
		_specifiedBy = option;
		_minimum = minimum;
		_maximum = maximum;
		_allowEqual = allowEqual;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (_isNegated)
		{
			return NegatedToString();
		}

		string? specialCases = (_allowEqual, _minimum, _maximum) switch
		{
			(true, 1, null) => "at least once",
			(false, 1, null) => "more than once",
			(true, 2, null) => "at least twice",
			(false, 2, null) => "more than twice",
			(true, _, 0) => "never",
			(true, 1, 1) => "exactly once",
			(true, null, 1) => "at most once",
			(false, null, 1) => "fewer than once",
			(true, 2, 2) => "exactly twice",
			(true, null, 2) => "at most twice",
			(false, null, 2) => "fewer than twice",
			(_, _, _) => null,
		};
		if (specialCases != null)
		{
			return specialCases;
		}

		if (_minimum == _maximum)
		{
			return $"exactly {ToTimesString(_minimum)}";
		}

		if (_maximum == null)
		{
			return _allowEqual ? $"at least {ToTimesString(_minimum)}" : $"more than {ToTimesString(_minimum)}";
		}

		if (_minimum == null)
		{
			return _allowEqual ? $"at most {ToTimesString(_maximum)}" : $"fewer than {ToTimesString(_maximum)}";
		}

		return $"between {_minimum} and {_maximum} times";
	}

	private string NegatedToString()
	{
		string? specialCases = (_allowEqual, _minimum, _maximum) switch
		{
			(true, 1, null) => "never",
			(true, _, 0) => "at least once",
			(true, null, 1) => "more than once",
			(_, _, _) => null,
		};
		if (specialCases != null)
		{
			return specialCases;
		}

		if (_minimum == _maximum)
		{
			return $"not exactly {ToTimesString(_minimum)}";
		}

		if (_maximum == null)
		{
			return _allowEqual ? $"fewer than {ToTimesString(_minimum)}" : $"at most {ToTimesString(_minimum)}";
		}

		if (_minimum == null)
		{
			return _allowEqual ? $"more than {ToTimesString(_maximum)}" : $"at least {ToTimesString(_maximum)}";
		}

		return $"not between {_minimum} and {_maximum} times";
	}

	/// <summary>
	///     Negates the quantifier.
	/// </summary>
	public void Negate() => _isNegated = !_isNegated;

	private static string ToTimesString(int? value)
		=> value switch
		{
			1 => "once",
			2 => "twice",
			_ => $"{value} times",
		};
}
