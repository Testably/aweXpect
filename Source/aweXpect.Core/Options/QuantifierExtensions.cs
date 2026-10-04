using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that count occurrences with a <see cref="Quantifier" />.
/// </summary>
/// <remarks>
///     Only one of these options can be specified, so <c>AtLeast(2).AtMost(5)</c> throws; specify a range with
///     <c>Between(2).And(5)</c> instead.
/// </remarks>
public static class QuantifierExtensions
{
	/// <summary>
	///     Verifies that it occurs at least…
	/// </summary>
	public static CountTimesResult<TResult> AtLeast<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			result.Options.AtLeast(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	public static TResult AtLeast<TResult>(this TResult result, Times minimum)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.AtLeast(minimum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs at most…
	/// </summary>
	public static CountTimesResult<TResult> AtMost<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			result.Options.AtMost(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	public static TResult AtMost<TResult>(this TResult result, Times maximum)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.AtMost(maximum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TResult> Between<TResult>(this TResult result, int minimum)
		where TResult : IOptionsProvider<Quantifier>
		=> new(maximum =>
		{
			result.Options.Between(minimum, maximum);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	public static TResult Exactly<TResult>(this TResult result, Times expected)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.Exactly(expected.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs fewer than…
	/// </summary>
	public static CountTimesResult<TResult> LessThan<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			result.Options.LessThan(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	public static TResult LessThan<TResult>(this TResult result, Times maximum)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.LessThan(maximum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it occurs more than…
	/// </summary>
	public static CountTimesResult<TResult> MoreThan<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
		=> new(value =>
		{
			result.Options.MoreThan(value);
			return result;
		});

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	public static TResult MoreThan<TResult>(this TResult result, Times minimum)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.MoreThan(minimum.Value);
		return result;
	}

	/// <summary>
	///     Verifies that it never occurs.
	/// </summary>
	public static TResult Never<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.Exactly(0, nameof(Never));
		return result;
	}

	/// <summary>
	///     Verifies that it occurs exactly once.
	/// </summary>
	public static TResult Once<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.Exactly(1, nameof(Once));
		return result;
	}

	/// <summary>
	///     Verifies that it occurs exactly twice.
	/// </summary>
	public static TResult Twice<TResult>(this TResult result)
		where TResult : IOptionsProvider<Quantifier>
	{
		result.Options.Exactly(2, nameof(Twice));
		return result;
	}
}
