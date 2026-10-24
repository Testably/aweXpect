using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the operators of a <see langword="string" /> property, e.g. <c>EqualTo(…)</c> or
///     <c>StartingWith(…)</c>, which continue on <typeparamref name="TThat" /> with an underlying value of type
///     <typeparamref name="TType" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="StringPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names, argument checks, options and
///     expectation texts. Implement the interface explicitly, so that its members do not show up next to the
///     operators.
/// </remarks>
public interface IStringPropertyResult<TType, TThat>
{
	/// <summary>
	///     Validates the arguments of the operators: receives the value and the name of its parameter, e.g.
	///     <c>expected</c> or <c>unexpected</c>, and throws for an invalid value.
	/// </summary>
	/// <remarks>
	///     Return <see langword="null" /> when the operators accept any value.
	/// </remarks>
	Action<string?, string>? Validation { get; }

	/// <summary>
	///     Adds the comparison of the property with the <paramref name="expected" /> value and returns the result that
	///     <paramref name="createResult" /> creates from the <see cref="ExpectationBuilder" />, the subject to continue
	///     on and the <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="options" /> decide how the property is compared and describe the comparison, see
	///     <see cref="StringEqualityOptions.AreConsideredEqual{TExpected}(string?, TExpected)" /> and
	///     <see cref="StringEqualityOptions.GetExpectation(string?, ExpectationGrammars)" />. The operator returns the
	///     result before the caller can add further options, e.g. <c>IgnoringCase()</c>, so only read them when the
	///     expectation is evaluated. When <paramref name="isNegated" /> is set, e.g. for <c>NotEqualTo(…)</c>, the
	///     comparison has to be inverted.
	/// </remarks>
	TResult Compare<TResult>(string? expected, StringEqualityOptions options, bool isNegated,
		Func<ExpectationBuilder, TThat, StringEqualityOptions, TResult> createResult);
}
