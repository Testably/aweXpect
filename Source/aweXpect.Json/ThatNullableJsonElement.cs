using System.Text.Json;
using aweXpect.Core;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="JsonElement" />? values.
/// </summary>
public static partial class ThatNullableJsonElement
{
	private static string DescribeValueKind(JsonElement? element, string it, ExpectationGrammars grammars)
		=> ThatJsonElement.DescribeValueKind(element.GetValueOrDefault(), it, grammars);
}
