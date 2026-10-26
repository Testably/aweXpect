using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Compares <typeparamref name="TElement" /> values within a <typeparamref name="TTolerance" />.
/// </summary>
/// <remarks>
///     An expectation starts with the value of <see cref="ObjectMatchType{TElement}.WithTolerance{TTolerance}" />, which
///     applies its default tolerance, and <see cref="Within(TTolerance)" /> returns a new value with the specified
///     tolerance, so that a stored value never changes.
/// </remarks>
[ImmutableOptions]
public sealed class ObjectToleranceMatchType<TElement, TTolerance> : ObjectMatchType<TElement>
{
	private readonly Func<TTolerance, TTolerance, TTolerance>? _absoluteTolerance;
	private readonly Comparison _comparison;
	private readonly TTolerance _tolerance = default!;
	private readonly string _unit;
	private WithinMatcher? _matcher;

	private ObjectToleranceMatchType(Comparison comparison)
	{
		_comparison = comparison;
		_unit = "";
	}

	private ObjectToleranceMatchType(Comparison comparison, TTolerance tolerance,
		Func<TTolerance, TTolerance, TTolerance>? absoluteTolerance, string unit) : base(nameof(Within))
	{
		_comparison = comparison;
		_tolerance = tolerance;
		_absoluteTolerance = absoluteTolerance;
		_unit = unit;
	}

	/// <inheritdoc />
	/// <remarks>
	///     A default tolerance can be customized, so the phrase is written whenever it is asked for, and names the
	///     tolerance only when it differs from the default value of <typeparamref name="TTolerance" />.
	/// </remarks>
	public override MatchPhrase Phrase
	{
		get
		{
			if (OptionName is not null)
			{
				return GetPhrase(_tolerance);
			}

			if (_comparison.DefaultTolerance is null)
			{
				return MatchPhrase.EqualTo;
			}

			TTolerance value = _comparison.DefaultTolerance();
			return EqualityComparer<TTolerance>.Default.Equals(value, default!)
				? MatchPhrase.EqualTo
				: GetPhrase(value);
		}
	}

	internal static ObjectToleranceMatchType<TElement, TTolerance> Create(
		Func<TElement, TElement, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string>? toString,
		Action<TTolerance>? validation,
		Func<TTolerance>? defaultTolerance)
	{
		isWithinTolerance.ThrowIfNull();
		return new ObjectToleranceMatchType<TElement, TTolerance>(
			new Comparison(isWithinTolerance, toString ?? ToleranceHelpers.Format, validation, defaultTolerance));
	}

	/// <inheritdoc />
	/// <remarks>
	///     A default tolerance can be customized, so it is read once for each evaluation, and outside an evaluation
	///     for every comparison, while a specified tolerance is the same for every evaluation. Without a default
	///     tolerance, the values are compared like <see cref="ObjectMatchType{TElement}.Default" />.<br />
	///     The matcher outside an evaluation reads the default tolerance itself, so it is created once, also for a
	///     starting value that every expectation shares, where a race only creates a second, equal matcher.
	/// </remarks>
	public override ObjectMatcher<TElement> ForEvaluation(MatchEnvironment environment)
	{
		if (OptionName is not null)
		{
			return _matcher ??= new WithinMatcher(this, _tolerance);
		}

		if (_comparison.DefaultTolerance is not { } defaultTolerance)
		{
			return Default.ForEvaluation(environment);
		}

		return environment.Evaluation is null
			? _matcher ??= new WithinMatcher(this, default!, true)
			: new WithinMatcher(this, defaultTolerance());
	}

	/// <summary>
	///     Returns the comparison within the <paramref name="tolerance" />.
	/// </summary>
	/// <remarks>
	///     It replaces an earlier tolerance, including a relative one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="tolerance" /> is negative or cannot be honoured by the comparison.
	/// </exception>
	public ObjectToleranceMatchType<TElement, TTolerance> Within(TTolerance tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		_comparison.Validation?.Invoke(tolerance);
		return new ObjectToleranceMatchType<TElement, TTolerance>(_comparison, tolerance, null, "");
	}

	/// <summary>
	///     Returns the comparison that interprets the tolerance relative to the expected value.
	/// </summary>
	/// <param name="absoluteTolerance">
	///     Calculates the absolute tolerance from the tolerance and the expected value.
	/// </param>
	/// <param name="unit">The unit appended to the tolerance in the expectation text, e.g. <c>%</c>.</param>
	/// <remarks>
	///     Applies only to expected values of type <typeparamref name="TTolerance" />. A later relative tolerance
	///     replaces the earlier one, and a later <see cref="Within(TTolerance)" /> replaces both.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     No tolerance was specified with <see cref="Within(TTolerance)" />.
	/// </exception>
	public ObjectToleranceMatchType<TElement, TTolerance> SetRelativeTolerance(
		Func<TTolerance, TTolerance, TTolerance> absoluteTolerance, string unit)
	{
		absoluteTolerance.ThrowIfNull();
		unit.ThrowIfNull();
		ToleranceHelpers.ThrowIfRelativeToleranceHasNoTolerance(OptionName is not null);
		return new ObjectToleranceMatchType<TElement, TTolerance>(_comparison, _tolerance, absoluteTolerance, unit);
	}

	private MatchPhrase GetPhrase(TTolerance tolerance)
	{
		string name = _comparison.Format(tolerance) + _unit;
		return MatchPhrase.EqualTo.WithSuffix(name).WithName(name);
	}

	/// <summary>
	///     The comparison that every value derived from the same starting value shares.
	/// </summary>
	private sealed class Comparison(
		Func<TElement, TElement, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> format,
		Action<TTolerance>? validation,
		Func<TTolerance>? defaultTolerance)
	{
		public Func<TElement, TElement, TTolerance, bool> IsWithinTolerance { get; } = isWithinTolerance;
		public Func<TTolerance, string> Format { get; } = format;
		public Action<TTolerance>? Validation { get; } = validation;
		public Func<TTolerance>? DefaultTolerance { get; } = defaultTolerance;
	}

	/// <remarks>
	///     A value that is not a <typeparamref name="TElement" /> never matches. The matcher reads the default tolerance
	///     for every comparison when it <paramref name="readsTolerance" />.
	/// </remarks>
	private sealed class WithinMatcher(
		ObjectToleranceMatchType<TElement, TTolerance> matchType,
		TTolerance tolerance,
		bool readsTolerance = false)
		: ObjectMatcher<TElement, TElement>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match(TElement actual, TElement expected)
			=> new(MatchResult.From(IsEqual(actual, expected)));

		private bool IsEqual(TElement actual, TElement expected)
		{
			if (actual is null || expected is null)
			{
				return actual is null && expected is null;
			}

			return matchType._comparison.IsWithinTolerance(actual, expected, ToleranceFor(expected));
		}

		private TTolerance ToleranceFor(TElement expected)
		{
			TTolerance value = readsTolerance ? matchType._comparison.DefaultTolerance!() : tolerance;
			return matchType._absoluteTolerance is not null && expected is TTolerance typedExpected
				? matchType._absoluteTolerance(value, typedExpected)
				: value;
		}
	}
}
