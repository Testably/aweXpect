using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class ExpectationBuilderExtensionsTests
{
	[Test]
	public async Task AddStringOccurrenceConstraint_ShouldCountTheOccurrences()
	{
		string subject = "a b a";

		async Task Act()
			=> await ContainsOccurrences(That(subject), "a", false).Exactly(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains "a" exactly 3 times,
			             but it contained "a" twice in "a b a"
			             """);
	}

	[Test]
	public async Task AddStringOccurrenceConstraint_WhenNegated_ShouldFailWhenTheValueOccurs()
	{
		string subject = "a b a";

		async Task Act()
			=> await ContainsOccurrences(That(subject), "a", true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain "a",
			             but it contained "a" at least once in "a b a"
			             """);
	}

	private static StringOccurrenceCountResult<string, IThat<string>> ContainsOccurrences(IThat<string> subject,
		string expected, bool isNegated)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(expected));
		return new StringOccurrenceCountResult<string, IThat<string>>(
			subject.Get().ExpectationBuilder.AddStringOccurrenceConstraint(expected, quantifier, options, isNegated),
			subject, quantifier, options);
	}
}
