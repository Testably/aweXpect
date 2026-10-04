namespace aweXpect.Core.Tests.Core;

public sealed class ExpectationGrammarsExtensionsTests
{
	[Theory]
	[InlineData(ExpectationGrammars.None, false)]
	[InlineData(ExpectationGrammars.Negated, true)]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural, false)]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsNegated_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsNegated();

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None, false)]
	[InlineData(ExpectationGrammars.Nested, true)]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Nested, true)]
	[InlineData(ExpectationGrammars.Negated | ExpectationGrammars.Plural, false)]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsNested_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsNested();

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None, false)]
	[InlineData(ExpectationGrammars.Plural, true)]
	[InlineData(ExpectationGrammars.Plural | ExpectationGrammars.Nested, true)]
	[InlineData(ExpectationGrammars.Negated | ExpectationGrammars.Nested, false)]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsPlural_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsPlural();

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None, ExpectationGrammars.Negated)]
	[InlineData(ExpectationGrammars.Plural, ExpectationGrammars.Plural | ExpectationGrammars.Negated)]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural,
		ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated)]
	public async Task Negate_ShouldAddNegated(ExpectationGrammars input, ExpectationGrammars expected)
	{
		ExpectationGrammars result = input.Negate();

		await That(result).IsEqualTo(expected);
	}

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
