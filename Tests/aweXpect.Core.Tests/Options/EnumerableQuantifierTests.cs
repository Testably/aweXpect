using System.Text;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class EnumerableQuantifierTests
{
	[Test]
	[Arguments(0, "it was empty")]
	[Arguments(3, "all 3 were")]
	public async Task All_AppendResult_WhenNegated_ShouldNameTheMatchingItems(int totalCount, string expected)
	{
		EnumerableQuantifier sut = EnumerableQuantifier.All();
		StringBuilder sb = new();

		sut.AppendResult(sb, ExpectationGrammars.Negated, "it", totalCount, 0, totalCount);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	public async Task All_AppendResult_WithVerb_ShouldUseTheVerb()
	{
		EnumerableQuantifier sut = EnumerableQuantifier.All();
		StringBuilder sb = new();

		sut.AppendResult(sb, ExpectationGrammars.None, "it", 2, 1, 3, "are");

		await That(sb.ToString()).IsEqualTo("only 2 of 3 are");
	}

	[Test]
	[Arguments(false, "has values of which at least 2 are even")]
	[Arguments(true, "has values of which fewer than 2 are even")]
	public async Task AppendExpectation_WhenNested_ShouldReplaceTheSeparator(bool isNegated, string expected)
	{
		EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(2);
		StringBuilder sb = new("has values that ");
		ExpectationGrammars grammars = isNegated
			? ExpectationGrammars.Nested | ExpectationGrammars.Negated
			: ExpectationGrammars.Nested;

		sut.AppendExpectation(sb, grammars, (builder, _) => builder.Append("are even"));

		await That(sb.ToString()).IsEqualTo(expected)
			.Because("'that at least 2 are' is not grammatical");
	}

	[Test]
	public async Task AppendExpectation_WhenNestedWithoutPrecedingText_ShouldNotReplaceTheSeparator()
	{
		EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(2);
		StringBuilder sb = new();

		sut.AppendExpectation(sb, ExpectationGrammars.Nested,
			(builder, _) => builder.Append("are even"));

		await That(sb.ToString()).IsEqualTo("at least 2 are even");
	}

	[Test]
	public async Task GetNegatedQuantifierContext_WithoutComplement_ShouldBeNone()
	{
		EnumerableQuantifier sut = EnumerableQuantifier.Between(1, 3);

		EnumerableQuantifier.QuantifierContexts result = sut.GetNegatedQuantifierContext();

		await That(result).IsEqualTo(EnumerableQuantifier.QuantifierContexts.None)
			.Because("a range with two bounds lists no items, so its negation lists none either");
	}

	[Test]
	public async Task GetQuantifierContext_WhenExactlyNull_ShouldBeNone()
	{
		EnumerableQuantifier sut = EnumerableQuantifier.Exactly(null);

		EnumerableQuantifier.QuantifierContexts result = sut.GetQuantifierContext();
		EnumerableQuantifier.QuantifierContexts negatedResult = sut.GetNegatedQuantifierContext();

		await That(result).IsEqualTo(EnumerableQuantifier.QuantifierContexts.None);
		await That(negatedResult).IsEqualTo(EnumerableQuantifier.QuantifierContexts.None)
			.Because("no item can explain why a null count matches no collection");
	}
}
