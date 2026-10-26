using System;
using aweXpect.Results;

namespace aweXpect.Json.Options;

/// <summary>
///     The <see cref="JsonOptions" /> directly after <see cref="JsonOptionsExtensions.ComparingNumbersWithin" />, whose
///     tolerance can still be made relative to the expected value, e.g. via <c>Percent()</c>.
/// </summary>
public sealed class JsonNumbersWithinOptions
	: JsonOptions, IWithinToleranceResult<JsonOptions, double>
{
	private readonly double _tolerance;

	internal JsonNumbersWithinOptions(JsonOptions options, double tolerance) : base(options)
	{
		_tolerance = tolerance;
		NumberTolerance = new JsonNumberTolerance(tolerance);
	}

	/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.SetRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
	/// <remarks>
	///     Returns new options, so that these options keep their absolute tolerance.
	/// </remarks>
	JsonOptions IWithinToleranceResult<JsonOptions, double>.SetRelativeTolerance(
		Func<double, double, double> absoluteTolerance, string unit)
		=> new(this)
		{
			NumberTolerance = new JsonNumberTolerance(_tolerance, absoluteTolerance, unit),
		};
}
