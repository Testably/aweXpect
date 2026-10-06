namespace aweXpect.Core.Tests.Core;

public sealed class ExpectationGrammarsExtensionsTests
{
	[Test]
	[Arguments(ExpectationGrammars.None, false)]
	[Arguments(ExpectationGrammars.Negated, true)]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural, false)]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsNegated_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsNegated();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None, false)]
	[Arguments(ExpectationGrammars.Nested, true)]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Nested, true)]
	[Arguments(ExpectationGrammars.Negated | ExpectationGrammars.Plural, false)]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsNested_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsNested();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None, false)]
	[Arguments(ExpectationGrammars.Plural, true)]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Nested, true)]
	[Arguments(ExpectationGrammars.Negated | ExpectationGrammars.Nested, false)]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated, true)]
	public async Task IsPlural_ShouldReturnExpectedValue(ExpectationGrammars input, bool expected)
	{
		bool result = input.IsPlural();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None, ExpectationGrammars.Negated)]
	[Arguments(ExpectationGrammars.Plural, ExpectationGrammars.Plural | ExpectationGrammars.Negated)]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural,
		ExpectationGrammars.Nested | ExpectationGrammars.Plural | ExpectationGrammars.Negated)]
	public async Task Negate_ShouldAddNegated(ExpectationGrammars input, ExpectationGrammars expected)
	{
		ExpectationGrammars result = input.Negate();

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None, "it", "was")]
	[Arguments(ExpectationGrammars.Plural, "it", "was")]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Nested, "it", "was")]
	[Arguments(ExpectationGrammars.None, "lines", "was")]
	[Arguments(ExpectationGrammars.Plural, "lines", "were")]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Negated, "lines", "were")]
	public async Task SubjectVerb_ShouldOnlyUsePluralForAMemberName(
		ExpectationGrammars input, string it, string expected)
	{
		string result = input.SubjectVerb(it, "was", "were");

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None, "has")]
	[Arguments(ExpectationGrammars.Nested, "has")]
	[Arguments(ExpectationGrammars.Negated, "has")]
	[Arguments(ExpectationGrammars.Plural, "have")]
	[Arguments(ExpectationGrammars.Plural | ExpectationGrammars.Nested, "have")]
	public async Task Verb_ShouldOnlyUsePluralWhenPluralFlagIsSet(ExpectationGrammars input, string expected)
	{
		string result = input.Verb("has", "have");

		await That(result).IsEqualTo(expected);
	}
}
