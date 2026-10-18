using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
	///     <see cref="JsonValueKind.False" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsBoolean(this IThat<JsonElement> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.True)),
			subject);
}
