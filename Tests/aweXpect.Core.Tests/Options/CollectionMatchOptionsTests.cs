using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionMatchOptionsTests
{
	public class AnyOrderTests
	{
		[Fact]
		public async Task Contains_WithinInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			double[] subject = [1.1, 1.0, 5.0,];

			async Task Act()
				=> await That(subject).Contains([1.0, 1.2,]).Within(0.15).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("1.1 matches 1.2 and 1.0 matches 1.0");
		}

		[Fact]
		public async Task IsEqualTo_PredicatesInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			int[] subject = [1, 2,];
			Expression<Func<int, bool>>[] expected = [x => x > 0, x => x == 1,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("2 matches x > 0 and 1 matches x == 1");
		}

		[Fact]
		public async Task IsEqualTo_WithinInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			double[] subject = [1.1, 1.0,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2,]).Within(0.15).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("1.1 matches 1.2 and 1.0 matches 1.0");
		}

		[Fact]
		public async Task IsEqualTo_WithinInAnyOrder_WhenNoValidAssignmentExists_ShouldReportTheUnassignedItems()
		{
			double[] subject = [1.1, 1.0, 1.15,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2, 3.0,]).Within(0.15).InAnyOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1.0, 1.2, 3.0,] ± 0.15 in any order,
				             but it
				               contained item 1.15 at index 2 that was not expected and
				               lacked 1 of 3 expected items: 3.0

				             Collection:
				             [1.1, 1.0, 1.15]

				             Expected:
				             [1.0, 1.2, 3.0]
				             """);
		}

		[Fact]
		public async Task IsEqualTo_WithinInAnyOrder_WhenMoreItemsThanDeviationsAllowedAreReassigned_ShouldSucceed()
		{
			// Each 10k + 0.5 takes 10k, the first expected item it matches, so each later 10k has to move it to 10k + 1.
			double[] subject = Enumerable.Range(0, 22).Select(k => (10 * k) + 0.5)
				.Concat(Enumerable.Range(0, 22).Select(k => 10.0 * k))
				.ToArray();
			double[] expected = Enumerable.Range(0, 22).SelectMany(k => new[] { 10.0 * k, (10 * k) + 1.0, })
				.ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected).Within(0.5).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("10k matches 10k and 10k + 0.5 matches 10k + 1");
		}
	}

	public class FailureMessageTests
	{
		[Fact]
		public async Task WhenAllOfManyExpectedItemsAreMissingInAnyOrder_ShouldCountThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(100, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in any order,
				             but it lacked all 30 expected items

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [
				               100,
				               101,
				               102,
				               103,
				               104,
				               105,
				               106,
				               107,
				               108,
				               109,
				               (… and 20 more)
				             ]
				             """)
				.Because("the missing items are known completely, so they are listed like fewer missing items");
		}

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
				             but it lacked 1 of 2 expected items: an item that is equal to "c"

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
		public async Task WhenExpectationItemThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["a", "b",];
			Action<IThat<string?>>[] expected =
			[
				x => x.IsEqualTo("a"),
				x => x.Satisfies(_ => throw exception),
			];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Expected:
				             [
				               an item that is equal to "a",
				               an item that satisfies _ => throw exception
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task WhenExpectationItemThrowsUnderNegation_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["a", "b",];
			Action<IThat<string?>>[] expected =
			[
				x => x.IsEqualTo("a"),
				x => x.Satisfies(_ => throw exception),
			];

			async Task Act()
				=> await That(subject).IsNotEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Expected:
				             [
				               an item that is equal to "a",
				               an item that satisfies _ => throw exception
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception))
				.Because("an item that the expectation did not answer fails the negation as well");
		}

		[Fact]
		public async Task WhenExpectationItemThrowsForAnItemThatMatchesAnotherExpectedItemInAnyOrder_ShouldSucceed()
		{
			string[] subject = ["b", "a",];
			Action<IThat<string?>>[] expected =
			[
				x => x.Satisfies(s => s == "a" ? true : throw new InvalidOperationException("boom")),
				x => x.IsEqualTo("b"),
			];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).DoesNotThrow();
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
				             is equal to collection expected using equivalency in order,
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

				             Equivalency options:
				              - include public fields and properties
				             """)
				.Because("an item and the expected item that format identically are only told apart by their type");
		}

		[Fact]
		public async Task WhenIncorrectItemIsALongStringThatDiffersAfterTheMaximumStringLength_ShouldShowTheDifference()
		{
			string common = new('a', 120);
			string[] subject = [common + "x",];
			string[] expected = [common + "y",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it contained item "…aaaaaaaaaax" at index 0 instead of "…aaaaaaaaaay"
				             *
				             """).AsWildcard()
				.Because("both truncated texts would be identical, so the strings are shown from shortly before their first difference");
		}

		[Fact]
		public async Task WhenManyExpectedItemsAreMissing_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but it lacked 25 of 30 expected items:
				               6,
				               7,
				               8,
				               9,
				               10,
				               11,
				               12,
				               13,
				               14,
				               15,
				               (… and 15 more)

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [
				               1,
				               2,
				               3,
				               4,
				               5,
				               6,
				               7,
				               8,
				               9,
				               10,
				               (… and 20 more)
				             ]
				             """)
				.Because("the missing items are known completely, so they are listed like fewer missing items");
		}

		[Fact]
		public async Task WhenManyExpectedItemsAreMissingIgnoringDuplicates_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous ignoring duplicates,
				             but it lacked 25 of 30 expected items:
				               6,
				               7,
				               8,
				               9,
				               10,
				               11,
				               12,
				               13,
				               14,
				               15,
				               (… and 15 more)

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [
				               1,
				               2,
				               3,
				               4,
				               5,
				               6,
				               7,
				               8,
				               9,
				               10,
				               (… and 20 more)
				             ]
				             """)
				.Because("the missing items are known completely, so they are listed like fewer missing items");
		}

		[Fact]
		public async Task WhenManyExpectedItemsAreMissingInAnyOrder_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in any order,
				             but it lacked 25 of 30 expected items:
				               6,
				               7,
				               8,
				               9,
				               10,
				               11,
				               12,
				               13,
				               14,
				               15,
				               (… and 15 more)

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [
				               1,
				               2,
				               3,
				               4,
				               5,
				               6,
				               7,
				               8,
				               9,
				               10,
				               (… and 20 more)
				             ]
				             """)
				.Because("the missing items are known completely, so they are listed like fewer missing items");
		}

		[Fact]
		public async Task WhenManyExpectedItemsAreMissingInAnyOrderIgnoringDuplicates_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in any order ignoring duplicates,
				             but it lacked 25 of 30 expected items:
				               6,
				               7,
				               8,
				               9,
				               10,
				               11,
				               12,
				               13,
				               14,
				               15,
				               (… and 15 more)

				             Collection:
				             [1, 2, 3, 4, 5]

				             Expected:
				             [
				               1,
				               2,
				               3,
				               4,
				               5,
				               6,
				               7,
				               8,
				               9,
				               10,
				               (… and 20 more)
				             ]
				             """)
				.Because("the missing items are known completely, so they are listed like fewer missing items");
		}

		[Fact]
		public async Task WhenPredicateThrowsInAnyOrder_AndTheItemIsAtAnotherPosition_ShouldNameTheIndexOfTheItem()
		{
			string[] subject = ["c", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard()
				.Because("the index names the subject item, not the position of the predicate that threw");
		}

		[Fact]
		public async Task WhenPredicateThrowsInAnyOrder_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard()
				.Because("the item that the predicate did not answer is named like in the other collection expectations");
		}

		[Fact]
		public async Task WhenPredicateThrowsInAnyOrderIgnoringDuplicates_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order ignoring duplicates,
				             but for the item at index 2, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task WhenPredicateThrowsInOrder_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard();
		}

		[Fact]
		public async Task WhenPredicateThrowsInOrderIgnoringDuplicates_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in order ignoring duplicates,
				             but for the item at index 2, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard()
				.Because("the duplicates are compared once, so the item is named by the index where it first occurs");
		}

		private static bool Throw(string _)
			=> throw new InvalidOperationException("boom");
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
			"is contained in collection [1] that has at least one additional item in order and contiguous")]
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

	public class IgnoringDuplicatesTests
	{
		[Fact]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateInAnyOrder()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["b", "a",]).InAnyOrder().IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateInSameOrder()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateWhenContained()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsContainedIn(["x", "a", "b", "y",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task
			WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateWhenContainedWithInterspersedItems()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsContainedIn(["x", "a", "y", "b",]).IgnoringInterspersedItems()
					.IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateWhenContaining()
		{
			string[] subject = ["x", "a", "A", "b",];

			async Task Act()
				=> await That(subject).Contains(["x", "a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasingWithoutIgnoringCase_ShouldNotBeADuplicate()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["b", "a",]).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection ["b", "a",] in any order ignoring duplicates,
				             but it contained item "A" at index 1 that was not expected

				             Collection:
				             [
				               "a",
				               "A",
				               "b"
				             ]

				             Expected:
				             [
				               "b",
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task WhenAnItemMatchesTwoExpectedValuesInAnyOrder_ShouldMatchBoth()
		{
			double[] subject = [1.1,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2,]).Within(0.15).InAnyOrder().IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("each expected value is matched by an item, as in the same order");
		}

		[Fact]
		public async Task WhenAnItemOnlyMatchesAnAlreadyMatchedExpectedValue_ShouldBeNoAdditionalItem()
		{
			string[] subject = ["a", "A",];

			async Task Act()
				=> await That(subject).Contains(["a",]).Properly().InAnyOrder().IgnoringDuplicates().IgnoringCase();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection ["a",] ignoring case and at least one additional item in any order ignoring duplicates,
				             but it did not contain any additional items

				             Collection:
				             [
				               "a",
				               "A"
				             ]

				             Expected:
				             [
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task WhenExpectedValuesDifferOnlyByCasing_ShouldBeDuplicatesInAnyOrder()
		{
			string[] subject = ["a",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "A",]).IgnoringDuplicates().InAnyOrder().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("\"a\" and \"A\" are duplicates when case is ignored");
		}

		[Fact]
		public async Task WhenOneItemMatchesTwoPredicatesInSameOrder_ShouldFail()
		{
			int[] subject = [1,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x > 0, x => x == 1,]).IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x > 0, x => x == 1,] in order ignoring duplicates,
				             but it lacked 1 of 2 expected items: x => (x == 1)

				             Collection:
				             [1]

				             Expected:
				             [
				               x => (x > 0),
				               x => (x == 1)
				             ]
				             """)
				.Because("each predicate needs its own distinct item, as in any order");
		}

		[Fact]
		public async Task WhenTheRunIsInterruptedInSameOrder_ShouldReportTheInterruptingItem()
		{
			int[] subject = [3, 0, 2, 2,];

			async Task Act()
				=> await That(subject).Contains([x => x >= 1, x => x == 2,]).IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [x => x >= 1, x => x == 2,] in order and contiguous ignoring duplicates,
				             but it contained item 0 at index 1 instead of x => (x == 2)

				             Collection:
				             [3, 0, 2, 2]

				             Expected:
				             [
				               x => (x >= 1),
				               x => (x == 2)
				             ]
				             """);
		}

		[Fact]
		public async Task WhenTheRepeatedItemFitsAPredicateInSameOrder_ShouldChooseIt()
		{
			int[] subject = [3, 3, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x <= 3, x => x >= 0,]).IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("3 matches x <= 3 and 2 matches x >= 0");
		}

		[Fact]
		public async Task WhenTwoItemsMatchTheSameExpectedValue_ShouldBeDuplicates()
		{
			string[] subject = ["abc", "axe",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a.*",]).AsRegex().IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Fact]
		public async Task WhenTwoItemsMatchTheSamePredicate_ShouldNotBeDuplicates()
		{
			int[] subject = [1, 4, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x > 0, x => x == 2,]).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x > 0, x => x == 2,] in any order ignoring duplicates,
				             but it contained item 4 at index 1 that was not expected

				             Collection:
				             [1, 4, 2]

				             Expected:
				             [
				               x => (x > 0),
				               x => (x == 2)
				             ]
				             """)
				.Because("a predicate can match unrelated items, so only equal items are duplicates");
		}
	}

	public class IgnoringInterspersedItemsTests
	{
		[Fact]
		public async Task WhenInAnyOrderIsSpecified_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.InAnyOrder();

			void Act() => sut.IgnoringInterspersedItems();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringInterspersedItems cannot be combined with InAnyOrder.")
				.Because("the any-order match never requires contiguous items, so the option would silently be dropped");
		}

		[Fact]
		public async Task WhenInAnyOrderIsSpecifiedAfterwards_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems();

			void Act() => sut.InAnyOrder();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("InAnyOrder cannot be combined with IgnoringInterspersedItems.")
				.Because("the any-order match never requires contiguous items, so the option would silently be dropped");
		}
	}

	/// <summary>
	///     Cases in which the matchers deviated from a brute-force comparison over small collections.
	/// </summary>
	public class ReferenceCaseTests
	{
		[Theory]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent, "any", "near", "1,3,3,0", "2,3,0,1", true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "any", "near", "1,3,3,0", "2,3,0,1",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "any", "near", "2,0,0,0", "1,2",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent, "any", "pred", "0,2,1", ">=0,<=2,<=0",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "any", "pred", "1,2", ">=3,<=3,<=1",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "any-dup", "pred", "0,2,3,1", "<=3,==2,<=0",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "any-dup", "div2", "1,3,3,0", "2,3,0,1",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "any-dup", "near", "1,2", "1",
			false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly, "any-dup", "div2", "1,3,3,0",
			"2,3,0,1", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same", "eq", "0,1", "1,0", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-interspersed", "eq", "0,2,3,3",
			"2,3,0", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "eq", "0,2,3,3", "2,3,0",
			false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "eq", "1,2,0,2", "1,1,0,0",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "same-dup", "eq", "2,0", "2,3,0,3",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "div2", "2,0,3,1", "1,0,2",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "div2", "2,0,3,1", "1,0,2",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "same-dup", "div2", "0,2,1", "0,3",
			false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly, "same-dup", "div2", "0",
			"1,0", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "pred", "0", "<=2,==0",
			false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "pred", "3,3,2", "<=3,>=0",
			true)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "pred", "0,0,1,3",
			"==3,<=2,<=2", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "pred", "3,0,2,2", ">=1,==2",
			false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup-interspersed", "pred", "2,0,3,1",
			"==0,>=2,>=3", false)]
		[InlineData(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "same-dup", "pred", "1,3,2,2",
			">=1,<=3,<=3", false)]
		public async Task ShouldAgreeWithTheReference(CollectionMatchOptions.EquivalenceRelations relation,
			string mode, string equality, string subject, string expected, bool isMatch)
		{
			CollectionMatchOptions sut = new(relation);
			if (mode is ['a', ..])
			{
				sut.InAnyOrder();
			}

			if (mode.Contains("dup"))
			{
				sut.IgnoringDuplicates();
			}

			if (mode.Contains("interspersed"))
			{
				sut.IgnoringInterspersedItems();
			}

			ICollectionMatcher<int, int> matcher = equality == "pred"
				? sut.GetCollectionMatcher<int, int>(expected.Split(',').Select(ToPredicate))
				: sut.GetCollectionMatcher<int, int>(expected.Split(',').Select(int.Parse));

			bool result = await Matches(matcher, subject.Split(',').Select(int.Parse), new Equality(equality));

			await That(result).IsEqualTo(isMatch);
		}

		[Fact]
		public async Task ContainmentInOrder_ShouldAgreeWithABruteForceSearch()
		{
			CollectionMatchOptions.EquivalenceRelations[] relations =
			[
				CollectionMatchOptions.EquivalenceRelations.Contains,
				CollectionMatchOptions.EquivalenceRelations.ContainsProperly,
				CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
				CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly,
			];
			string[] equalities = ["eq", "div2", "near", "pred",];
			Random random = new(1530);
			List<string> disagreements = new();
			for (int run = 0; run < 2000; run++)
			{
				CollectionMatchOptions.EquivalenceRelations relation = relations[random.Next(relations.Length)];
				bool isInterspersed = random.Next(2) == 0;
				string equality = equalities[random.Next(equalities.Length)];
				int[] subject = Enumerable.Range(0, random.Next(0, 6)).Select(_ => random.Next(4)).ToArray();
				string[] expected = Enumerable.Range(0, random.Next(1, 5))
					.Select(_ => (equality == "pred" ? new[] { "==", ">=", "<=", }[random.Next(3)] : "") + random.Next(4))
					.ToArray();
				CollectionMatchOptions sut = new(relation);
				if (isInterspersed)
				{
					sut.IgnoringInterspersedItems();
				}

				ICollectionMatcher<int, int> matcher = equality == "pred"
					? sut.GetCollectionMatcher<int, int>(expected.Select(ToPredicate))
					: sut.GetCollectionMatcher<int, int>(expected.Select(int.Parse));
				bool result = await Matches(matcher, subject, new Equality(equality));
				if (result != IsContainedInOrder(relation, isInterspersed, subject, expected,
					    (value, expectedText) => IsMatch(equality, value, expectedText)))
				{
					disagreements.Add($"{relation} {(isInterspersed ? "interspersed " : "")}{equality} " +
					                  $"[{string.Join(",", subject)}] vs [{string.Join(",", expected)}]: {result}");
				}
			}

			await That(disagreements).IsEmpty();
		}

		private static bool IsMatch(string equality, int value, string expected)
			=> equality switch
			{
				"pred" => ToPredicate(expected).Compile()(value),
				"div2" => value / 2 == int.Parse(expected) / 2,
				"near" => Math.Abs(value - int.Parse(expected)) <= 1,
				_ => value == int.Parse(expected),
			};

		/// <summary>
		///     Searches every offset for a run, or the earliest matching items for a subsequence, which finds one whenever
		///     there is one.
		/// </summary>
		private static bool IsContainedInOrder(CollectionMatchOptions.EquivalenceRelations relation,
			bool isInterspersed, int[] subject, string[] expected, Func<int, string, bool> isMatch)
		{
			bool isContains = relation.HasFlag(CollectionMatchOptions.EquivalenceRelations.Contains);
			int searchedCount = isContains ? subject.Length : expected.Length;
			int soughtCount = isContains ? expected.Length : subject.Length;
			bool Fits(int searched, int sought)
				=> isContains ? isMatch(subject[searched], expected[sought]) : isMatch(subject[sought], expected[searched]);

			bool isFound;
			if (isInterspersed)
			{
				int position = 0;
				for (int sought = 0; sought < soughtCount && position <= searchedCount; sought++, position++)
				{
					while (position < searchedCount && !Fits(position, sought))
					{
						position++;
					}
				}

				isFound = position <= searchedCount;
			}
			else
			{
				isFound = Enumerable.Range(0, Math.Max(0, searchedCount - soughtCount + 1))
					.Any(offset => Enumerable.Range(0, soughtCount).All(sought => Fits(offset + sought, sought)));
			}

			return isFound && (relation switch
			{
				CollectionMatchOptions.EquivalenceRelations.ContainsProperly => subject.Length > expected.Length,
				CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly => subject.Length < expected.Length,
				_ => true,
			});
		}

		private static async Task<bool> Matches(ICollectionMatcher<int, int> matcher, IEnumerable<int> subject,
			IOptionsEquality<int> options)
		{
			foreach (int item in subject)
			{
				(bool isFailure, string? _) = await matcher.Verify("it", item, options, 10);
				if (isFailure)
				{
					return false;
				}
			}

			(bool isCompleteFailure, string? error) = await matcher.VerifyComplete("it", options, 10);
			await That(error is not null).IsEqualTo(isCompleteFailure)
				.Because("each failure is described");
			return !isCompleteFailure;
		}

		/// <remarks>
		///     The value is a constant, so that equal predicates are duplicates.
		/// </remarks>
		private static Expression<Func<int, bool>> ToPredicate(string text)
		{
			ParameterExpression x = Expression.Parameter(typeof(int), "x");
			ConstantExpression value = Expression.Constant(int.Parse(text.Substring(2)));
			Expression body = text.Substring(0, 2) switch
			{
				"==" => Expression.Equal(x, value),
				">=" => Expression.GreaterThanOrEqual(x, value),
				_ => Expression.LessThanOrEqual(x, value),
			};
			return Expression.Lambda<Func<int, bool>>(body, x);
		}

		/// <summary>
		///     "div2" merges distinct values like ignoring the casing; "near" is not transitive, like a tolerance.
		/// </summary>
		private sealed class Equality(string kind) : IOptionsEquality<int>
		{
			public ValueTask<bool> AreConsideredEqual<TExpected>(int actual, TExpected expected)
				=> new(expected is int value && kind switch
				{
					"div2" => actual / 2 == value / 2,
					"near" => Math.Abs(actual - value) <= 1,
					_ => actual == value,
				});
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
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatch_ShouldReportTheItemsBeforeTheMatchAsAdditional()
		{
			int[] subject = [1, 2, 5, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [2, 3,] in order,
				             but it
				               contained item 1 at index 0 that was not expected and
				               contained item 2 at index 1 that was not expected and
				               contained item 5 at index 2 that was not expected and
				               contained item 2 at index 3 that was not expected

				             Collection:
				             [1, 2, 5, 2, 2, 3]

				             Expected:
				             [2, 3]
				             """)
				.Because("leaving out the items before the match is the alignment with the fewest deviations");
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
				               contained item 1 at index 0 that was not expected and
				               contained item 2 at index 2 that was not expected

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
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldReportTheItemsBeforeTheMatchAsAdditional()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [2, 3,] in order,
				             but it
				               contained item 1 at index 0 that was not expected and
				               contained item 2 at index 1 that was not expected

				             Collection:
				             [1, 2, 2, 3]

				             Expected:
				             [2, 3]
				             """)
				.Because("leaving out the items before the match is the alignment with the fewest deviations");
		}

		[Fact]
		public async Task
			WhenTheMatchWouldRestartAtARepeatedItemIgnoringDuplicates_ShouldSkipTheRepeatedItem()
		{
			string[] subject = ["x", "a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection ["a", "b",] ignoring case in order ignoring duplicates,
				             but it contained item "x" at index 0 that was not expected

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
				             """)
				.Because("an item that matches an already matched expected value repeats it instead of restarting the match");
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
