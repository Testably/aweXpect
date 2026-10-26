using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that count occurrences with a <see cref="Quantifier" />.
/// </summary>
/// <remarks>
///     Only one of these options can be specified, so <c>AtLeast(2).AtMost(5)</c> throws; specify a range with
///     <c>Between(2).And(5)</c> instead.<br />
///     A result that implements <see cref="ICopyOnOption{TOptions, TSelf}" /> stays unchanged: it returns a new one
///     with the new quantifier.
/// </remarks>
public static class QuantifierExtensions
{
	/// <summary>
	///     Verifies that it occurs at least…
	/// </summary>
	public static CountTimesResult<TSelf> AtLeast<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
			{
				return copyOnOption.WithOptions(result.Options.Value.AtLeast(value));
			}

			result.Options.AtLeast(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	public static TSelf AtLeast<TSelf>(this TSelf result, Times minimum)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.AtLeast(minimum.Value));
		}

		result.Options.AtLeast(minimum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs at most…
	/// </summary>
	public static CountTimesResult<TSelf> AtMost<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
			{
				return copyOnOption.WithOptions(result.Options.Value.AtMost(value));
			}

			result.Options.AtMost(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	public static TSelf AtMost<TSelf>(this TSelf result, Times maximum)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.AtMost(maximum.Value));
		}

		result.Options.AtMost(maximum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TSelf, Times> Between<TSelf>(this TSelf result, int minimum)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(maximum =>
		{
			if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
			{
				return copyOnOption.WithOptions(result.Options.Value.Between(minimum, maximum));
			}

			result.Options.Between(minimum, maximum);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	public static TSelf Exactly<TSelf>(this TSelf result, Times expected)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.Exactly(expected.Value));
		}

		result.Options.Exactly(expected.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs fewer than…
	/// </summary>
	public static CountTimesResult<TSelf> LessThan<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
			{
				return copyOnOption.WithOptions(result.Options.Value.LessThan(value));
			}

			result.Options.LessThan(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	public static TSelf LessThan<TSelf>(this TSelf result, Times maximum)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.LessThan(maximum.Value));
		}

		result.Options.LessThan(maximum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs more than…
	/// </summary>
	public static CountTimesResult<TSelf> MoreThan<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
			{
				return copyOnOption.WithOptions(result.Options.Value.MoreThan(value));
			}

			result.Options.MoreThan(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	public static TSelf MoreThan<TSelf>(this TSelf result, Times minimum)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.MoreThan(minimum.Value));
		}

		result.Options.MoreThan(minimum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it never occurs.
	/// </summary>
	public static TSelf Never<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.Exactly(0, QuantifierOption.Never));
		}

		result.Options.Exactly(0, QuantifierOption.Never);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs exactly once.
	/// </summary>
	public static TSelf Once<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.Exactly(1, QuantifierOption.Once));
		}

		result.Options.Exactly(1, QuantifierOption.Once);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs exactly twice.
	/// </summary>
	public static TSelf Twice<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
	{
		if (result is ICopyOnOption<Quantifier, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.Exactly(2, QuantifierOption.Twice));
		}

		result.Options.Exactly(2, QuantifierOption.Twice);
		return result;
	}
}
