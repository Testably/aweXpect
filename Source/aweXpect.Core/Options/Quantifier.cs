using System;
using aweXpect.Core.Helpers;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace aweXpect.Options;

/// <summary>
///     Quantifier an occurrence.
/// </summary>
public class Quantifier
{
	private bool _allowEqual = true;
	private bool _isNegated;
	private int? _maximum;
	private int? _minimum = 1;

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
	///     Verifies, that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	public void AtLeast(int minimum)
	{
		if (minimum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(minimum),
					"The parameter 'minimum' must be non-negative")
				.LogTrace();
		}

		_minimum = minimum;
		_maximum = null;
		_allowEqual = true;
	}

	/// <summary>
	///     Verifies, that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	public void AtMost(int maximum)
	{
		if (maximum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximum),
					"The parameter 'maximum' must be non-negative")
				.LogTrace();
		}

		_minimum = null;
		_maximum = maximum;
		_allowEqual = true;
	}

	/// <summary>
	///     Verifies, that it occurs between <paramref name="minimum" /> and <paramref name="maximum" /> times.
	/// </summary>
	public void Between(int minimum, int maximum)
	{
		if (minimum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(minimum),
					"The parameter 'minimum' must be non-negative")
				.LogTrace();
		}

		if (maximum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximum),
					"The parameter 'maximum' must be non-negative")
				.LogTrace();
		}

		if (minimum > maximum)
		{
			throw new ArgumentException("The parameter 'maximum' must be greater than or equal to 'minimum'")
				.LogTrace();
		}

		_minimum = minimum;
		_maximum = maximum;
		_allowEqual = true;
	}

	/// <summary>
	///     Verifies, that it occurs less than <paramref name="maximum" /> times.
	/// </summary>
	public void LessThan(int maximum)
	{
		if (maximum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximum),
					"The parameter 'maximum' must be non-negative")
				.LogTrace();
		}

		_minimum = null;
		_maximum = maximum;
		_allowEqual = false;
	}

	/// <summary>
	///     Verifies, that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	public void MoreThan(int minimum)
	{
		if (minimum < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(minimum),
					"The parameter 'minimum' must be non-negative")
				.LogTrace();
		}

		_minimum = minimum;
		_maximum = null;
		_allowEqual = false;
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
	///     Verifies, that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	public void Exactly(int expected)
	{
		if (expected < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(expected),
					"The parameter 'expected' must be non-negative")
				.LogTrace();
		}

		_minimum = expected;
		_maximum = expected;
		_allowEqual = true;
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
			(false, null, 1) => "less than once",
			(true, 2, 2) => "exactly twice",
			(true, null, 2) => "at most twice",
			(false, null, 2) => "less than twice",
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
			return _allowEqual ? $"at most {ToTimesString(_maximum)}" : $"less than {ToTimesString(_maximum)}";
		}

		return $"between {_minimum} and {_maximum} times";
	}

	private string NegatedToString()
	{
		string? specialCases = (_allowEqual, _minimum, _maximum) switch
		{
			(true, 1, null) => "never",
			(true, _, 0) => "at least once",
			(true, 1, 1) => "not once",
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
			return _allowEqual ? $"less than {ToTimesString(_minimum)}" : $"at most {ToTimesString(_minimum)}";
		}

		if (_minimum == null)
		{
			return _allowEqual ? $"more than {ToTimesString(_maximum)}" : $"at least {ToTimesString(_maximum)}";
		}

		return $"outside {_minimum} and {_maximum} times";
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
