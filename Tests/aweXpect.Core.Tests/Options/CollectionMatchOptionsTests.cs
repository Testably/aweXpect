using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionMatchOptionsTests
{
	public class FailureMessageTests
	{
		[Fact]
		public async Task WhenAllOfOneExpectedItemIsMissing_ShouldUseSingular()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [3,] in order,
				             but it lacked the 1 expected item

				             Collection:
				             [1, 2]

				             Expected:
				             [3]
				             """);
		}

		[Fact]
		public async Task WhenAllOfOneUniqueExpectedItemIsMissing_ShouldUseSingular()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3, 3,]).IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [3, 3,] in order ignoring duplicates,
				             but it lacked the 1 unique expected item

				             Collection:
				             [1, 2]

				             Expected:
				             [3, 3]
				             """);
		}

		[Fact]
		public async Task WhenAllOfTwoExpectedItemsAreMissing_ShouldUsePlural()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3, 4,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [3, 4,] in order,
				             but it lacked all 2 expected items

				             Collection:
				             [1, 2]

				             Expected:
				             [3, 4]
				             """);
		}

		[Fact]
		public async Task WhenExpectationItemIsNotMet_ShouldDescribeItAsAnItem()
		{
			string[] subject = ["a", "b",];
			Action<IThat<string?>>[] expected =
			[
				x => x.IsEqualTo("a"),
				x => x.IsEqualTo("c"),
			];

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order,
				             but it
				               contained item "b" at index 1 instead of an item that is equal to "c" and
				               lacked 1 of 2 expected items: an item that is equal to "c"

				             Collection:
				             [
				               "a",
				               "b"
				             ]

				             Expected:
				             [
				               an item that is equal to "a",
				               an item that is equal to "c"
				             ]
				             """);
		}
	}

	public class GetExpectationTests
	{
		[Theory]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.None, "is equal to collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural, "are equal to collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Negated, "is not equal to collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated, "are not equal to collection [1] in order")]
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
