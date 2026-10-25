using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the quantified <c>Containing(…)</c> operator of a <see langword="string" /> property, which
///     continues with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operator is an extension method in <see cref="StringPropertyResultExtensions" />, so that every result that
///     implements this interface offers it with the same name, argument checks and expectation text. Implement the
///     interface explicitly, so that its members do not show up next to the operator.
///     <para />
///     <typeparamref name="TResult" /> usually offers the quantifier of the
///     <see cref="StringPropertyComparison.Quantifier" />, e.g. <c>AtLeast(2)</c>, and the string options, e.g.
///     <see cref="StringOccurrenceCountResult{TType, TThat}" />.
/// </remarks>
public interface IStringOccurrencePropertyResult<out TResult>
{
	/// <summary>
	///     Validates the argument of the operator: receives the value and the name of its parameter, i.e.
	///     <c>expected</c>, and throws for an invalid value.
	/// </summary>
	/// <remarks>
	///     Return <see langword="null" /> when the operator accepts any value.
	/// </remarks>
	Action<string?, string>? Validation { get; }

	/// <summary>
	///     Adds the <paramref name="comparison" /> of the property and returns the result to continue with.
	/// </summary>
	TResult Compare(StringPropertyComparison comparison);
}
