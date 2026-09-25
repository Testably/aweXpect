using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Checks equality of objects with an optional tolerance.
/// </summary>
public class ObjectEqualityWithToleranceOptions<TSubject, TTolerance>(
	Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
	Func<TTolerance, string>? toString = null)
	: ObjectEqualityOptions<TSubject>
{
	/// <summary>
	///     Specifies the <paramref name="tolerance" /> within which the actual value is considered equal to the expected
	///     value.
	/// </summary>
	public ObjectEqualityOptions<TSubject> Within(TTolerance tolerance)
	{
		ThrowIfToleranceIsInvalid(tolerance);
		MatchType = new WithinMatchType(() => tolerance, false, isWithinTolerance,
			toString ?? DefaultToleranceFormatter);
		return this;
	}

	/// <summary>
	///     Specifies the <paramref name="defaultTolerance" /> that applies until a tolerance is specified with
	///     <see cref="Within(TTolerance)" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="defaultTolerance" /> is read when the comparison is made or described, or once per
	///     evaluation through <see cref="ForEvaluation()" />, and it is only named in the expectation text when it
	///     differs from the default value of <typeparamref name="TTolerance" />.
	/// </remarks>
	public ObjectEqualityWithToleranceOptions<TSubject, TTolerance> WithDefaultTolerance(
		Func<TTolerance> defaultTolerance)
	{
		MatchType = new WithinMatchType(defaultTolerance, true, isWithinTolerance,
			toString ?? DefaultToleranceFormatter);
		return this;
	}

	/// <inheritdoc />
	public override IOptionsEquality<TSubject> ForEvaluation()
	{
		if (MatchType is not WithinMatchType { IsDefault: true, } matchType)
		{
			return this;
		}

		ObjectEqualityOptions<TSubject> options = new();
		options.SetMatchType(matchType.WithResolvedTolerance());
		return options;
	}

	/// <summary>
	///     Rejects the same tolerances as <see cref="NumberTolerance{TNumber}" /> and <see cref="TimeTolerance" />,
	///     which a comparison with a tolerance can never honour.
	/// </summary>
	/// <remarks>
	///     The tolerance is generic here, so both checks are made against the runtime value: only a floating point
	///     number can be NaN, and a negative value is one that compares below the default of its own type, which
	///     exists only for a value type.
	/// </remarks>
	private static void ThrowIfToleranceIsInvalid(TTolerance tolerance)
	{
		if ((tolerance is double doubleTolerance && double.IsNaN(doubleTolerance)) ||
		    (tolerance is float floatTolerance && float.IsNaN(floatTolerance)))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must not be NaN"));
		}

		if (default(TTolerance) is { } zero && tolerance is IComparable<TTolerance> comparable &&
		    comparable.CompareTo(zero) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be non-negative"));
		}
	}

	private static string DefaultToleranceFormatter(TTolerance tolerance)
		=> $" ± {Formatter.Format(tolerance)}";

	private sealed class WithinMatchType(
		Func<TTolerance> tolerance,
		bool isDefault,
		Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> toString)
		: IObjectMatchType
	{
		public bool IsDefault => isDefault;

		public WithinMatchType WithResolvedTolerance()
		{
			TTolerance value = tolerance();
			return new WithinMatchType(() => value, isDefault, isWithinTolerance, toString);
		}

		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
#if NET8_0_OR_GREATER
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return ValueTask.FromResult(true);
			}

			return ValueTask.FromResult(actual is TSubject typedActual && expected is TSubject typedExpected &&
			                            isWithinTolerance(typedActual, typedExpected, tolerance()));
		}
#else
		public Task<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return Task.FromResult(true);
			}

			return Task.FromResult(actual is TSubject typedActual && expected is TSubject typedExpected &&
			                       isWithinTolerance(typedActual, typedExpected, tolerance()));
		}
#endif

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented())}";

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
		{
			TTolerance value = tolerance();
			return isDefault && EqualityComparer<TTolerance>.Default.Equals(value, default!)
				? ""
				: toString.Invoke(value);
		}

		#endregion
	}
}
