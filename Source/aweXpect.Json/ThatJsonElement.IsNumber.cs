using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber(this IThat<JsonElement> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement> (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Number)),
			subject);
}
