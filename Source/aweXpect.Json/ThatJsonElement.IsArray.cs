using System.Linq;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	public static IsArrayResult<JsonElement, IThat<JsonElement>, JsonElement> IsArray(this IThat<JsonElement> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement> (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Array)),
			subject,
			static element => element.EnumerateArray().ToArray());
}
