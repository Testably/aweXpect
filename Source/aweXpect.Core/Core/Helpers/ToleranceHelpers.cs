using System;
using System.Text;

namespace aweXpect.Core.Helpers;

/// <summary>
///     The validation and the text of a tolerance, shared by the tolerances of numbers, times and item comparisons.
/// </summary>
internal static class ToleranceHelpers
{
	/// <summary>
	///     Rejects a NaN or negative <paramref name="tolerance" />, which a comparison can never honour.
	/// </summary>
	/// <remarks>
	///     Only a floating point number can be NaN, and a negative value is one that compares below the default of its
	///     own type.
	/// </remarks>
	public static void ThrowIfInvalid<TTolerance>(TTolerance tolerance)
		=> ThrowIfInvalid(tolerance,
			(tolerance is double doubleTolerance && double.IsNaN(doubleTolerance)) ||
			(tolerance is float floatTolerance && float.IsNaN(floatTolerance)));

	/// <summary>
	///     Rejects a <paramref name="tolerance" /> that <paramref name="isNaN" /> or is negative.
	/// </summary>
	public static void ThrowIfInvalid<TTolerance>(TTolerance tolerance, bool isNaN)
	{
		if (isNaN)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "The tolerance must not be NaN."));
		}

		if (default(TTolerance) is { } zero && tolerance is IComparable<TTolerance> comparable &&
		    comparable.CompareTo(zero) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "The tolerance must not be negative."));
		}
	}

	/// <summary>
	///     Returns the text of the <paramref name="tolerance" />, e.g. <c> ± 0.1</c>.
	/// </summary>
	public static string Format<TTolerance>(TTolerance tolerance)
		=> Append(new StringBuilder(), tolerance).ToString();

	/// <summary>
	///     Appends the text of the <paramref name="tolerance" />, e.g. <c> ± 0.1</c>.
	/// </summary>
	/// <remarks>
	///     A <see langword="char" /> tolerance is a distance between code points and would be unreadable as a character.
	/// </remarks>
	public static StringBuilder Append<TTolerance>(StringBuilder stringBuilder, TTolerance tolerance)
	{
		stringBuilder.Append(" ± ");
		if (tolerance is char character)
		{
			Formatter.Format(stringBuilder, (int)character);
		}
		else
		{
			Formatter.Format(stringBuilder, tolerance);
		}

		return stringBuilder;
	}
}
