using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class GrammarHelpersTests
{
	[Theory]
	[InlineData(ExpectationGrammars.None, "it", "was")]
	[InlineData(ExpectationGrammars.Plural, "it", "was")]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Nested, "it", "was")]
	[InlineData(ExpectationGrammars.None, "lines", "was")]
	[InlineData(ExpectationGrammars.Plural, "lines", "were")]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Negated, "lines", "were")]
	public async Task SubjectVerb_ShouldOnlyUsePluralForAMemberName(
		ExpectationGrammars input, string it, string expected)
	{
		string result = input.SubjectVerb(it, "was", "were");

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None, "has")]
	[InlineData(ExpectationGrammars.Nested, "has")]
	[InlineData(ExpectationGrammars.Negated, "has")]
	[InlineData(ExpectationGrammars.Plural, "have")]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Nested, "have")]
	public async Task Verb_ShouldOnlyUsePluralWhenPluralFlagIsSet(ExpectationGrammars input, string expected)
	{
		string result = input.Verb("has", "have");

		await That(result).IsEqualTo(expected);
	}
}
