using System;
using System.Globalization;
using aweXpect.Options;

namespace aweXpect.Json;

/// <summary>
///     The tolerance of <see cref="JsonOptionsExtensions.ComparingNumbersWithin" />, which is never changed once created.
/// </summary>
/// <remarks>
///     The tolerance is rendered like a JSON number, e.g. <c> ± 5%</c>, as aweXpect would render a
///     <see langword="double" /> tolerance as <c> ± 5.0%</c>, unlike the compared JSON numbers.
/// </remarks>
internal sealed class JsonNumberTolerance
{
	private readonly NumberTolerance<double> _numberTolerance;

	private readonly string _text;

	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="tolerance" /> is negative or NaN.</exception>
	public JsonNumberTolerance(double tolerance, Func<double, double, double>? absoluteTolerance = null,
		string unit = "")
	{
		NumberTolerance<double> numberTolerance =
			new NumberTolerance<double>(static (actual, expected) => Math.Abs(actual - expected)).Within(tolerance);
		_numberTolerance = absoluteTolerance is null
			? numberTolerance
			: numberTolerance.SetRelativeTolerance(absoluteTolerance, unit);
		_text = " ± " + tolerance.ToString("R", CultureInfo.InvariantCulture) + unit;
	}

	public bool IsWithinTolerance(double actual, double expected)
		=> _numberTolerance.IsWithinTolerance(actual, expected);

	public override string ToString() => _text;
}
