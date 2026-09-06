using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionMatchOptionsTests
{
	public class GetExpectationTests
	{
		[Theory]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.None, "matches collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural, "match collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Negated, "does not match collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated, "do not match collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.None, "contains collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural, "contain collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated, "do not contain collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.None, "is contained in collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.Plural, "are contained in collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated, "are not contained in collection [1] in order")]
		public async Task ShouldAgreeWithTheNumberOfTheSubject(
			CollectionMatchOptions.EquivalenceRelations equivalenceRelations,
			ExpectationGrammars grammars,
			string expected)
		{
			CollectionMatchOptions sut = new(equivalenceRelations);

			string result = sut.GetExpectation("[1]", grammars);

			await That(result).IsEqualTo(expected);
		}
	}

	public class EquivalenceRelationsTests
	{
		[Fact]
		public async Task Equivalent_ShouldNotHaveSubsetOrSupersetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.Equivalent;

			await That(subject).DoesNotHaveFlag(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
			await That(subject).DoesNotHaveFlag(CollectionMatchOptions.EquivalenceRelations.Contains);
		}

		[Fact]
		public async Task ProperSubset_ShouldHaveSubsetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly;

			await That(subject).HasFlag(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		}

		[Fact]
		public async Task ProperSuperset_ShouldHaveSupersetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.ContainsProperly;

			await That(subject).HasFlag(CollectionMatchOptions.EquivalenceRelations.Contains);
		}
	}
}
