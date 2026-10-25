using System.Text.Json;
using aweXpect.Core;

namespace aweXpect.Json;

/// <summary>
///     Formats and compares the kinds of JSON values.
/// </summary>
internal static class JsonValidation
{
	internal static string Format(JsonValueKind valueKind, ExpectationGrammars grammars = ExpectationGrammars.None)
	{
		string kind = valueKind switch
		{
			JsonValueKind.Array => "an array",
			JsonValueKind.Object => "an object",
			JsonValueKind.Number => "a number",
			JsonValueKind.String => "a string",
			_ => valueKind.ToString().ToLower(),
		};
		return grammars.IsNegated() ? "not " + kind : kind;
	}

	/// <summary>
	///     Checks if the <paramref name="actual" /> kind is the <paramref name="expected" /> kind, where an expected
	///     <see cref="JsonValueKind.True" /> stands for a boolean and is therefore also met by
	///     <see cref="JsonValueKind.False" />.
	/// </summary>
	internal static bool IsKind(JsonValueKind actual, JsonValueKind expected)
		=> actual == expected || (expected == JsonValueKind.True && actual == JsonValueKind.False);

	/// <summary>
	///     Formats the <paramref name="expected" /> kind, where <see cref="JsonValueKind.True" /> stands for a boolean.
	/// </summary>
	internal static string FormatExpected(JsonValueKind expected, ExpectationGrammars grammars)
	{
		if (expected != JsonValueKind.True)
		{
			return Format(expected, grammars);
		}

		return grammars.IsNegated() ? "not a boolean" : "a boolean";
	}
}
