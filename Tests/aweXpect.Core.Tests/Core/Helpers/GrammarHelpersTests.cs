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
	public async Task SubjectVerb_ShouldOnlyUsePluralForAReplacedSubject(
		ExpectationGrammars grammars, string it, string expected)
	{
		string result = grammars.SubjectVerb(it, "was", "were");

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None, "has")]
	[InlineData(ExpectationGrammars.Nested, "has")]
	[InlineData(ExpectationGrammars.Negated, "has")]
	[InlineData(ExpectationGrammars.Plural, "have")]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Nested, "have")]
	public async Task Verb_ShouldUsePluralOnlyWhenPluralFlagIsSet(ExpectationGrammars grammars, string expected)
	{
		string result = grammars.Verb("has", "have");

		await That(result).IsEqualTo(expected);
	}
}
