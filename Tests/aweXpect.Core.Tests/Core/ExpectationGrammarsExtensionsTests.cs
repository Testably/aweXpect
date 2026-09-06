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
}
