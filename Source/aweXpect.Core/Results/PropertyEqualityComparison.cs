using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System;
#endif

namespace aweXpect.Results;

/// <summary>
///     The equality comparison of a <typeparamref name="TNumber" /> property within a tolerance, that an operator like
///     <c>EqualTo(…)</c> passes to the <c>CompareEquality</c> method of a property result, e.g. of an
///     <see cref="IDoublePropertyResult{TResult, TToleranceResult}" />.
/// </summary>
/// <remarks>
///     The tolerance is set on the result after the operator returned it, e.g. <c>Within(0.1)</c>, so pass the
///     <see cref="Options" /> to the result you return, e.g. to a
///     <see cref="NumberToleranceResult{TType, TThat, TNumber}" />. The comparison applies the tolerance whenever it is
///     evaluated, also several times, and formats the expectation text only when a message needs it.
/// </remarks>
public sealed class PropertyEqualityComparison<TNumber> : IPropertyComparison<TNumber>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>
#else
	where TNumber : struct, IComparable<TNumber>
#endif
{
	private readonly TNumber? _expected;

	internal PropertyEqualityComparison(TNumber? expected, OptionsHolder<NumberTolerance<TNumber>> options, bool isNegated)
	{
		_expected = expected;
		Options = options;
		IsNegated = isNegated;
	}

	/// <summary>
	///     The holder with the tolerance of the comparison, which the result you return provides to <c>Within(…)</c>.
	/// </summary>
	public OptionsHolder<NumberTolerance<TNumber>> Options { get; }

	/// <summary>
	///     Flag indicating that the operator is a negated one, i.e. <c>NotEqualTo(…)</c>.
	/// </summary>
	/// <remarks>
	///     <see cref="Evaluate(TNumber?)" /> already includes the negation, but
	///     <see cref="AppendExpectation(StringBuilder)" /> does not, so that the "not" can be placed where the sentence
	///     needs it, e.g. "has rating not equal to 4.5" or "does not have rating equal to 4.5".
	/// </remarks>
	public bool IsNegated { get; }

	/// <summary>
	///     Appends the comparison without its negation, e.g. "equal to 4.5 ± 0.1", to the
	///     <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder)
	{
		stringBuilder.Append("equal to ");
		Formatter.Format(stringBuilder, _expected);
		stringBuilder.Append(Options);
	}

	/// <summary>
	///     Evaluates the comparison against the <paramref name="actual" /> value of the property.
	/// </summary>
	/// <remarks>
	///     The values are compared like <c>IsEqualTo(…).Within(…)</c> does: <c>NaN</c> is equal to <c>NaN</c>, an
	///     infinite value only to the same infinite value, and <see langword="null" /> only to
	///     <see langword="null" />.
	/// </remarks>
	public Outcome Evaluate(TNumber? actual)
		=> Options.Value.IsWithinTolerance(actual, _expected) != IsNegated ? Outcome.Success : Outcome.Failure;
}
