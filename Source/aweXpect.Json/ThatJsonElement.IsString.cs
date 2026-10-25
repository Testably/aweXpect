using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
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
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static IAsyncContextConstraint<JsonElement> (state, it, grammars)
					=> new IsJsonStringEqualToConstraint(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	[GenerateExpectation("IsString", "is {not} a string",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.String\" />.")]
	private static bool IsStringKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.String;
}
