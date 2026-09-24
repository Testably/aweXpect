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
				             contains collection [3,] in order and contiguous,
				             but it lacked the one expected item

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
				             contains collection [3, 3,] in order and contiguous ignoring duplicates,
				             but it lacked the one unique expected item

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
				             contains collection [3, 4,] in order and contiguous,
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
				             contains collection expected in order and contiguous,
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

		[Fact]
		public async Task WhenIncorrectItemFormatsLikeTheExpectedItem_ShouldIncludeTheRuntimeType()
		{
			object[] subject = [0, 1,];
			object[] expected = [0, 1L,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).Equivalent();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order using equivalency,
				             but it contained item 1 (int) at index 1 instead of 1 (long)

				             Collection:
				             [
				               0,
				               1
				             ]

				             Expected:
				             [
				               0,
				               1
				             ]
				             """)
				.Because("an item and the expected item that format identically are only told apart by their type");
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
			ExpectationGrammars.None, "contains collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural, "contain collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not contain collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.None, "is contained in collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.Plural, "are contained in collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"are not contained in collection [1] in order and contiguous")]
		public async Task ShouldAgreeWithTheNumberOfTheSubject(
			CollectionMatchOptions.EquivalenceRelations equivalenceRelations,
			ExpectationGrammars grammars,
			string expected)
		{
			CollectionMatchOptions sut = new(equivalenceRelations);

			string result = sut.GetExpectation("[1]", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Theory]
		[InlineData(false, false, false, "contains collection [1] in order and contiguous")]
		[InlineData(false, false, true, "contains collection [1] in order ignoring interspersed items")]
		[InlineData(false, true, false, "contains collection [1] in order and contiguous ignoring duplicates")]
		[InlineData(false, true, true, "contains collection [1] in order ignoring duplicates and interspersed items")]
		[InlineData(true, false, false, "contains collection [1] in any order")]
		[InlineData(true, true, false, "contains collection [1] in any order ignoring duplicates")]
		public async Task ShouldClaimContiguityDependingOnTheOptions(
			bool inAnyOrder,
			bool ignoringDuplicates,
			bool ignoringInterspersedItems,
			string expected)
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			if (inAnyOrder)
			{
				sut.InAnyOrder();
			}

			if (ignoringDuplicates)
			{
				sut.IgnoringDuplicates();
			}

			if (ignoringInterspersedItems)
			{
				sut.IgnoringInterspersedItems();
			}

			string result = sut.GetExpectation("[1]", ExpectationGrammars.None);

			await That(result).IsEqualTo(expected)
				.Because("contiguity may only be claimed while interspersed items are not ignored");
		}

		[Theory]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			"is equal to collection [1] in order")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains,
			"contains collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.ContainsProperly,
			"contains collection [1] and at least one additional item in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			"is contained in collection [1] in order and contiguous")]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly,
			"is contained in collection [1] which has at least one additional item in order and contiguous")]
		public async Task ShouldOnlyClaimContiguityForTheContainmentRelations(
			CollectionMatchOptions.EquivalenceRelations equivalenceRelations,
			string expected)
		{
			CollectionMatchOptions sut = new(equivalenceRelations);

			string result = sut.GetExpectation("[1]", ExpectationGrammars.None);

			await That(result).IsEqualTo(expected)
				.Because("only the containment relations forbid other items in between");
		}
	}

	public class RestartedMatchTests
	{
		[Fact]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatch_ShouldBeContained()
		{
			int[] subject = [1, 2, 4, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatch_ShouldReportEachItemAtItsPosition()
		{
			int[] subject = [1, 2, 5, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [2, 3,] in order,
				             but it
				               contained item 1 at index 0 instead of 2 and
				               contained item 2 at index 1 instead of 3 and
				               contained item 5 at index 2 that was not expected and
				               contained item 2 at index 3 that was not expected and
				               contained item 2 at index 4 that was not expected and
				               contained item 3 at index 5 that was not expected

				             Collection:
				             [1, 2, 5, 2, 2, 3]

				             Expected:
				             [2, 3]
				             """)
				.Because("equality compares each item with the expected item at its position instead of restarting the match");
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatchIgnoringDuplicates_ShouldBeContained()
		{
			int[] subject = [1, 2, 4, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]).IgnoringDuplicates();

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldBeContained()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldNotBeContainedIn()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsContainedIn([2, 3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [2, 3,] in order and contiguous,
				             but it
				               contained item 1 at index 0 instead of 2 and
				               contained item 2 at index 1 instead of 3 and
				               contained item 2 at index 2 that was not expected and
				               contained item 3 at index 3 that was not expected

				             Collection:
				             [1, 2, 2, 3]

				             Expected:
				             [2, 3]
				             """);
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldNotBeEqual()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsNotEqualTo([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldReportEachItemAtItsPosition()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [2, 3,] in order,
				             but it
				               contained item 1 at index 0 instead of 2 and
				               contained item 2 at index 1 instead of 3 and
				               contained item 2 at index 2 that was not expected and
				               contained item 3 at index 3 that was not expected

				             Collection:
				             [1, 2, 2, 3]

				             Expected:
				             [2, 3]
				             """)
				.Because("equality compares each item with the expected item at its position instead of restarting the match");
		}

		[Fact]
		public async Task
			WhenTheMatchRestartsAfterAPartialMatchIgnoringDuplicates_ShouldReportTheAbandonedItemsAsAdditional()
		{
			string[] subject = ["x", "a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection ["a", "b",] in order ignoring duplicates ignoring case,
				             but it
				               contained item "x" at index 0 that was not expected and
				               contained item "a" at index 1 that was not expected

				             Collection:
				             [
				               "x",
				               "a",
				               "A",
				               "b"
				             ]

				             Expected:
				             [
				               "a",
				               "b"
				             ]
				             """);
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
