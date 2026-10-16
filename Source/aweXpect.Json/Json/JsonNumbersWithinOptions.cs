using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Json;

/// <summary>
///     The <see cref="JsonOptions" /> directly after <see cref="JsonOptionsExtensions.NumbersWithin" />, whose tolerance
///     can still be made relative to the expected value, e.g. via <c>Percent()</c>.
/// </summary>
public sealed record JsonNumbersWithinOptions
	: JsonOptions, IWithinToleranceResult<JsonOptions, double>, IRelativeToleranceOptions<double>
{
	private readonly double _tolerance;

	internal JsonNumbersWithinOptions(JsonOptions options, double tolerance) : base(options)
	{
		_tolerance = tolerance;
		NumberTolerance = new JsonNumberTolerance(tolerance);
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	IRelativeToleranceOptions<double> IOptionsProvider<IRelativeToleranceOptions<double>>.Options => this;

	/// <inheritdoc cref="IWithinToleranceResult{TResult,TTolerance}.Result" />
	JsonOptions IWithinToleranceResult<JsonOptions, double>.Result => this;

	/// <inheritdoc cref="IRelativeToleranceOptions{TTolerance}.UseRelativeTolerance(Func{TTolerance, TTolerance, TTolerance}, string)" />
	/// <remarks>
	///     Replaces the tolerance instead of changing it, so that the copies of these options keep their tolerance.
	/// </remarks>
	void IRelativeToleranceOptions<double>.UseRelativeTolerance(Func<double, double, double> absoluteTolerance,
		string unit)
		=> NumberTolerance = new JsonNumberTolerance(_tolerance, absoluteTolerance, unit);
}
