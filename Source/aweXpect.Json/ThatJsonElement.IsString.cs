using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.String" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsString(this IThat<JsonElement> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.String)),
			source);
}
