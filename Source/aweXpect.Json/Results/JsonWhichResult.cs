using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     An <see cref="AndOrResult{TType,TThat}" /> for JSON strings.
/// </summary>
/// <remarks>
///     <see cref="Which" /> parses the string with the options passed to <see cref="ThatJsonString.IsValidJson" />,
///     so the result can only be created by it.
/// </remarks>
public sealed class JsonWhichResult : AndOrResult<string?, IThat<string?>>
{
	private readonly JsonDocumentOptions _options;

	internal JsonWhichResult(ExpectationBuilder expectationBuilder, IThat<string?> returnValue,
		JsonDocumentOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

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
					using JsonDocument jsonDocument = JsonDocument.Parse(jsonString, _options);
					return jsonDocument.RootElement.Clone();
				}, "it"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<JsonElement?>(e)));
		return this;
	}
}
