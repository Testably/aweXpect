using System;
using aweXpect.Core;

namespace aweXpect.Json;

/// <summary>
///     Holds the comparison that an operator on the result of an expectation like <c>IsString()</c> hands to the
///     constraint that the expectation already added, so that the value is checked in one sentence, e.g.
///     <c>is a string starting with "Let"</c>.
/// </summary>
/// <param name="expectation">The expectation whose result offers the operators, e.g. <c>IsString()</c>.</param>
internal sealed class JsonComparisonSlot<TComparison>(string expectation)
	where TComparison : class
{
	/// <summary>
	///     The comparison of the operator, or <see langword="null" /> when no operator was called.
	/// </summary>
	public TComparison? Comparison { get; private set; }

	/// <summary>
	///     Stores the <paramref name="comparison" /> of an operator.
	/// </summary>
	/// <exception cref="InvalidOperationException">An operator was already called on the result.</exception>
	public void Set(TComparison comparison)
	{
		if (Comparison is not null)
		{
			throw Tracing.WriteException(new InvalidOperationException(
				$"{expectation} accepts only one comparison, so call only one operator on its result."));
		}

		Comparison = comparison;
	}
}
