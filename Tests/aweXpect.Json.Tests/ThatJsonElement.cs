using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public const string SupportedNumberTypes =
#if NET8_0_OR_GREATER
		"byte, sbyte, short, ushort, int, uint, long, ulong, nint, nuint, Int128, UInt128, BigInteger, Half, float, double or decimal";
#else
		"byte, sbyte, short, ushort, int, uint, long, ulong, BigInteger, float, double or decimal";
#endif

	public static JsonElement FromString(string value)
	{
		using JsonDocument document = JsonDocument.Parse(value);
		return document.RootElement.Clone();
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the <paramref name="subject" /> as a plural member named
	///     "Items", which is the subject of the result text instead of "it".
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> WhosePluralItems(IThat<JsonElement> subject,
		Action<IThat<JsonElement>> expectations)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(MemberAccessor<JsonElement, JsonElement>.FromFuncWithName(items => items, "Items"),
				(_, stringBuilder) => stringBuilder.Append("whose Items "))
			.AddExpectations(e => expectations(new ThatSubject<JsonElement>(e)),
				grammars => grammars | ExpectationGrammars.Plural);
		return new AndOrResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject);
	}
}
