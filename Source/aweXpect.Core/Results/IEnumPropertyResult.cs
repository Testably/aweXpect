using System;

namespace aweXpect.Results;

/// <summary>
///     A result that offers the equality operators of a <typeparamref name="TEnum" /> property, <c>EqualTo(…)</c> and
///     <c>NotEqualTo(…)</c>, which continue with <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     The operators are extension methods in <see cref="EnumPropertyResultExtensions" />, so that every result that
///     implements this interface offers the same operators with the same names and expectation texts.
///     Implement the interface explicitly, so that its members do not show up next to the operators.
/// </remarks>
public interface IEnumPropertyResult<out TResult, TEnum>
	where TEnum : struct, Enum
{
	/// <summary>
	///     Adds the <paramref name="comparison" /> of the property and returns the result to continue with.
	/// </summary>
	TResult Compare(PropertyComparison<TEnum> comparison);
}
