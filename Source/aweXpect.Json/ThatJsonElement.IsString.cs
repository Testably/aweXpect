using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.String" /> whose value
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsString(this IThat<JsonElement> subject,
		Action<IThatSubject<string>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.String, expectations);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.String" /> equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The match type and the other string options are specified on the result, e.g. <c>AsPrefix()</c> or
	///     <c>IgnoringCase()</c>. A value of another kind fails the expectation, but satisfies its negation.
	/// </remarks>
	public static StringEqualityTypeResult<JsonElement, IThat<JsonElement>> IsString(this IThat<JsonElement> subject,
		string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) => new IsJsonStringEqualToConstraint(it, grammars, expected),
				out IsJsonStringEqualToConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.String" />.
	/// </summary>
	/// <remarks>
	///     The value can be compared on the result in the same expectation, e.g. with <c>StartingWith("Let")</c>,
	///     <c>EqualTo("Let It Be").IgnoringCase()</c> or <c>Containing("e").AtLeast(2)</c>. A value of another kind fails
	///     the expectation, but satisfies its negation.
	/// </remarks>
	public static IsStringResult<JsonElement, IThat<JsonElement>> IsString(this IThat<JsonElement> subject)
	{
		JsonComparisonSlot<StringPropertyComparison> slot = new("IsString()");
		return new IsStringResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot,
				static IAsyncContextConstraint<JsonElement> (slot, it, grammars)
					=> new IsJsonStringConstraint(it, grammars, slot)),
			subject,
			slot);
	}
}
