using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is <see cref="JsonValueKind.Null" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNull(this IThat<JsonElement> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement> (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Null)),
			subject);
}
