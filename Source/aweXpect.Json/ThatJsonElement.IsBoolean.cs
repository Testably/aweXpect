using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
	///     <see cref="JsonValueKind.False" />, whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsBoolean(this IThat<JsonElement> subject,
		Action<IThatSubject<bool>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Boolean, expectations);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
	///     <see cref="JsonValueKind.False" />, equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A value of another kind fails the expectation, but satisfies its negation.
	/// </remarks>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsBoolean(this IThat<JsonElement> subject,
		bool expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(expected,
				static IAsyncContextConstraint<JsonElement> (expected, it, grammars)
					=> new IsJsonBooleanEqualToConstraint(it, grammars, expected)),
			subject);

	[GenerateExpectation("IsBoolean", "is {not} a boolean",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = """
		          Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
		          <see cref="JsonValueKind.False" />.
		          """)]
	private static bool IsBooleanKind(JsonElement element)
		=> element.ValueKind is JsonValueKind.True or JsonValueKind.False;
}
