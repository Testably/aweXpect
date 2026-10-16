using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyTypeOptionsExtensionsTests
{
	[Test]
	public async Task Comparing_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		EquivalencyOptions options = new();

		_ = options.Comparing<int, string>((number, text) => number.ToString() == text);

		await That(options.ToString()).IsEqualTo(" - include public fields and properties")
			.Because("the customized default is shared by every expectation");
	}

	[Test]
	public async Task Comparing_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> new EquivalencyOptions().Comparing<int, string>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task ToString_WhenComparing_ShouldListTheTypesAndThePredicate()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.Comparing<int, string>((number, text) => number.ToString() == text);

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - compare int with string by (number, text) => number.ToString() == text
		                             """);
	}

	[Test]
	public async Task ToString_WhenComparingForAType_ShouldListItBelowTheType()
	{
		EquivalencyOptions options = new EquivalencyOptions()
			.For<Version>(o => o.Comparing<int, string>((number, text) => number.ToString() == text));

		string result = options.ToString();

		await That(result).IsEqualTo("""
		                              - include public fields and properties
		                              - for Version:
		                                - include public fields and properties
		                                - compare int with string by (number, text) => number.ToString() == text
		                             """);
	}
}
