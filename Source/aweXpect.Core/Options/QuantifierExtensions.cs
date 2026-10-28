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
		=> new(value => OptionsProvider.WithOptions(result, result.Options.Value.AtLeast(value)));

	/// <summary>
	///     Verifies that it occurs at least <paramref name="minimum" /> times.
	/// </summary>
	public static TSelf AtLeast<TSelf>(this TSelf result, Times minimum)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.AtLeast(minimum.Value));

	/// <summary>
	///     Verifies that it occurs at most…
	/// </summary>
	public static CountTimesResult<TSelf> AtMost<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value => OptionsProvider.WithOptions(result, result.Options.Value.AtMost(value)));

	/// <summary>
	///     Verifies that it occurs at most <paramref name="maximum" /> times.
	/// </summary>
	public static TSelf AtMost<TSelf>(this TSelf result, Times maximum)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.AtMost(maximum.Value));

	/// <summary>
	///     Verifies that it occurs between <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<TSelf, Times> Between<TSelf>(this TSelf result, int minimum)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(maximum => OptionsProvider.WithOptions(result, result.Options.Value.Between(minimum, maximum)));

	/// <summary>
	///     Verifies that it occurs exactly <paramref name="expected" /> times.
	/// </summary>
	public static TSelf Exactly<TSelf>(this TSelf result, Times expected)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.Exactly(expected.Value));

	/// <summary>
	///     Verifies that it occurs fewer than…
	/// </summary>
	public static CountTimesResult<TSelf> LessThan<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value => OptionsProvider.WithOptions(result, result.Options.Value.LessThan(value)));

	/// <summary>
	///     Verifies that it occurs fewer than <paramref name="maximum" /> times.
	/// </summary>
	public static TSelf LessThan<TSelf>(this TSelf result, Times maximum)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.LessThan(maximum.Value));

	/// <summary>
	///     Verifies that it occurs more than…
	/// </summary>
	public static CountTimesResult<TSelf> MoreThan<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> new(value => OptionsProvider.WithOptions(result, result.Options.Value.MoreThan(value)));

	/// <summary>
	///     Verifies that it occurs more than <paramref name="minimum" /> times.
	/// </summary>
	public static TSelf MoreThan<TSelf>(this TSelf result, Times minimum)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.MoreThan(minimum.Value));

	/// <summary>
	///     Verifies that it never occurs.
	/// </summary>
	public static TSelf Never<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.Exactly(0, QuantifierOption.Never));

	/// <summary>
	///     Verifies that it occurs exactly once.
	/// </summary>
	public static TSelf Once<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.Exactly(1, QuantifierOption.Once));

	/// <summary>
	///     Verifies that it occurs exactly twice.
	/// </summary>
	public static TSelf Twice<TSelf>(this TSelf result)
		where TSelf : IOptionsProvider<Quantifier>
		=> OptionsProvider.WithOptions(result, result.Options.Value.Exactly(2, QuantifierOption.Twice));
}
