using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;

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
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see cref="object" />s.
	/// </summary>
	public ObjectEqualityOptions<TSubject> Within(TTolerance tolerance)
	{
		ThrowIfToleranceIsInvalid(tolerance);
		MatchType = new WithinMatchType(tolerance, isWithinTolerance, toString ?? DefaultToleranceFormatter);
		return this;
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
		TTolerance tolerance,
		Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string> toString)
		: IObjectMatchType
	{
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
			                            isWithinTolerance(typedActual, typedExpected, tolerance));
		}
#else
		public Task<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		{
			if (actual is null && expected is null)
			{
				return Task.FromResult(true);
			}

			return Task.FromResult(actual is TSubject typedActual && expected is TSubject typedExpected &&
			                       isWithinTolerance(typedActual, typedExpected, tolerance));
		}
#endif

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"is {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> $"{it} was {Formatter.Format(actual, FormattingOptions.Indented())}";

		/// <inheritdoc cref="IObjectMatchType.GetItemExpectation(string, string?, string?)" />
		public string GetItemExpectation(string expected, string? itemNoun = null, string? comparison = null)
			=> (comparison is null ? expected : $"{comparison} {expected}") + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> toString.Invoke(tolerance);

		#endregion
	}
}
