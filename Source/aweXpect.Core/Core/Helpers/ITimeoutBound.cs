using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Options whose upper bound also limits the evaluation, like a timeout added with
///     <see cref="ExpectationBuilder.WithTimeout(TimeSpan)" />.
/// </summary>
/// <remarks>
///     The upper bound is read when the evaluation starts, so that a later option replaces the bound of an earlier one.
/// </remarks>
internal interface ITimeoutBound
{
	/// <summary>
	///     The upper bound of the options, or <see langword="null" /> if they have none.
	/// </summary>
	TimeSpan? UpperBound { get; }
}
