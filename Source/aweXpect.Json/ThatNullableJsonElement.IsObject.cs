using System.Linq;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<JsonElement, IThat<JsonElement?>, JsonProperty> IsObject(
		this IThat<JsonElement?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement?> (it, grammar) =>
				new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Object)),
			subject,
			static element => element.EnumerateObject().ToArray());
}
