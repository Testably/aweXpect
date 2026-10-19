using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Results;

/// <summary>
///     An <see cref="AndOrResult{TType,TThat}" /> for JSON strings.
/// </summary>
public class JsonWhichResult(
	ExpectationBuilder expectationBuilder,
	IThat<string?> returnValue,
	JsonDocumentOptions options) : AndOrResult<string?, IThat<string?>>(expectationBuilder, returnValue)
{
	/// <summary>
	///     Allows specifying <paramref name="expectations" /> on the <see cref="JsonElement" />
	///     represented by the <see langword="string" />.
	/// </summary>
	public AndOrResult<string?, IThat<string?>> Which(Action<IThat<JsonElement?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder
			.ForMember(MemberAccessor<string, JsonElement?>.FromFunc(jsonString =>
				{
					using JsonDocument jsonDocument = JsonDocument.Parse(jsonString, options);
					return jsonDocument.RootElement.Clone();
				}, "it"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(e => expectations(new ThatSubject<JsonElement?>(e)));
		return this;
	}
}
