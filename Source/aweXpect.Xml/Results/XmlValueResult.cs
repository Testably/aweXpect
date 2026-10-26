using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Xml.Results;

/// <summary>
///     The result for verifying an XML node with a value, e.g. an attribute, which offers the operators of a
///     <see langword="string" /> property, e.g. <c>StartingWith("Let")</c>, to compare its value in the same expectation.
/// </summary>
/// <remarks>
///     Only one operator can be called on this result. Its expected value cannot be <see langword="null" />, as the
///     value of an XML node never is.
///     <para />
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class XmlValueResult<TType, TThat> : AndOrResult<TType, TThat>,
	IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>,
	IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>,
	IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>
{
	private static readonly Action<string?, string> ThrowIfNull = ThrowHelper.ThrowIfNull;

	private readonly TThat _returnValue;
	private readonly XmlComparisonSlot _slot;

	internal XmlValueResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		XmlComparisonSlot slot)
		: base(expectationBuilder, returnValue)
	{
		_returnValue = returnValue;
		_slot = slot;
	}

	/// <inheritdoc cref="IStringAffixPropertyResult{TResult}.Validation" />
	Action<string?, string>? IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>.Validation
		=> ThrowIfNull;

	/// <inheritdoc cref="IStringAffixPropertyResult{TResult}.Compare(StringPropertyComparison)" />
	StringEqualityResult<TType, TThat> IStringAffixPropertyResult<StringEqualityResult<TType, TThat>>.Compare(
		StringPropertyComparison comparison)
	{
		_slot.Set(comparison);
		return new StringEqualityResult<TType, TThat>(ExpectationBuilder, _returnValue, comparison.Options);
	}

	/// <inheritdoc cref="IStringEqualityPropertyResult{TResult}.Validation" />
	Action<string?, string>? IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>.Validation
		=> ThrowIfNull;

	/// <inheritdoc cref="IStringEqualityPropertyResult{TResult}.Compare(StringPropertyComparison)" />
	StringEqualityTypeResult<TType, TThat> IStringEqualityPropertyResult<StringEqualityTypeResult<TType, TThat>>.
		Compare(StringPropertyComparison comparison)
	{
		_slot.Set(comparison);
		return new StringEqualityTypeResult<TType, TThat>(ExpectationBuilder, _returnValue, comparison.Options);
	}

	/// <inheritdoc cref="IStringOccurrencePropertyResult{TResult}.Validation" />
	Action<string?, string>? IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>.Validation
		=> ThrowIfNull;

	/// <inheritdoc cref="IStringOccurrencePropertyResult{TResult}.Compare(StringPropertyComparison)" />
	StringOccurrenceCountResult<TType, TThat>
		IStringOccurrencePropertyResult<StringOccurrenceCountResult<TType, TThat>>.Compare(
			StringPropertyComparison comparison)
	{
		_slot.Set(comparison);
		return new StringOccurrenceCountResult<TType, TThat>(ExpectationBuilder, _returnValue,
			comparison.Quantifier!, comparison.Options);
	}
}
