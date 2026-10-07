using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CollectionMatchOptionsTests
{
	public class AnyOrderTests
	{
		[Test]
		public async Task Contains_PredicatesInAnyOrder_WhenAnItemOnlyMatchesAnAssignedPredicate_ShouldStopOnceDecided()
		{
			int readItems = 0;

			IEnumerable<int> Source()
			{
				readItems++;
				yield return 6;
				for (int i = 0; i < 10000; i++)
				{
					readItems++;
					yield return 3;
				}
			}

			async Task Act()
				=> await That(Source()).Contains([x => x > 0, x => x > 5,]).InAnyOrder();

			await That(Act).DoesNotThrow();
			await That(readItems).IsEqualTo(2)
				.Because("6 and 3 already satisfy both predicates, when 6 moves from x > 0 to x > 5");
		}

		[Test]
		public async Task Contains_WithinInAnyOrder_WhenAnItemOnlyMatchesAnAssignedValue_ShouldStopOnceDecided()
		{
			int readItems = 0;

			IEnumerable<double> Source()
			{
				for (int i = 0; i < 10000; i++)
				{
					readItems++;
					yield return i % 2 == 0 ? 1.1 : 0.95;
				}
			}

			async Task Act()
				=> await That(Source()).Contains([1.0, 1.2,]).Within(0.15).InAnyOrder();

			await That(Act).DoesNotThrow();
			await That(readItems).IsEqualTo(2)
				.Because("1.1 and 0.95 already match both values, when 1.1 moves from 1.0 to 1.2");
		}

		[Test]
		public async Task Contains_WithinInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			double[] subject = [1.1, 1.0, 5.0,];

			async Task Act()
				=> await That(subject).Contains([1.0, 1.2,]).Within(0.15).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("1.1 matches 1.2 and 1.0 matches 1.0");
		}

		[Test]
		public async Task IsEqualTo_PredicatesInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			int[] subject = [1, 2,];
			Expression<Func<int, bool>>[] expected = [x => x > 0, x => x == 1,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("2 matches x > 0 and 1 matches x == 1");
		}

		[Test]
		public async Task IsEqualTo_WithinInAnyOrder_WhenAValidAssignmentExists_ShouldSucceed()
		{
			double[] subject = [1.1, 1.0,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2,]).Within(0.15).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("1.1 matches 1.2 and 1.0 matches 1.0");
		}

		[Test]
		public async Task IsEqualTo_WithinInAnyOrder_WhenNoValidAssignmentExists_ShouldReportTheUnassignedItems()
		{
			double[] subject = [1.1, 1.0, 1.15,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2, 3.0,]).Within(0.15).InAnyOrder();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task IsEqualTo_WithinInAnyOrder_WhenMoreItemsThanDeviationsAllowedAreReassigned_ShouldSucceed()
		{
			// Each 10k + 0.5 takes 10k, the first expected item it matches, so each later 10k has to move it to 10k + 1.
			double[] subject = Enumerable.Range(0, 22).Select(k => (10 * k) + 0.5)
				.Concat(Enumerable.Range(0, 22).Select(k => 10.0 * k))
				.ToArray();
			double[] expected = Enumerable.Range(0, 22).SelectMany(k => new[]
				{
					10.0 * k, (10 * k) + 1.0,
				})
				.ToArray();

			async Task Act()
				=> await That(subject).IsEqualTo(expected).Within(0.5).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("10k matches 10k and 10k + 0.5 matches 10k + 1");
		}
	}

	/// <summary>
	///     Options that compare strings ordinally or primitives by their default equality agree with the same options
	///     behind another type, which the matcher cannot recognize.
	/// </summary>
	public class DefaultEqualityInAnyOrderTests
	{
		[Test]
		public async Task FloatingPointNumbers_ShouldAgreeWithUnrecognizedOptions()
		{
			double[] values = [0.0, -0.0, double.NaN, 1.0,];

			(int met, List<string> disagreements) = await Compare(1811,
				random => values[random.Next(values.Length)], new ObjectEqualityOptions<double>());

			await That(disagreements).IsEmpty();
			await That(met).IsGreaterThan(100);
		}

		[Test]
		public async Task Numbers_ShouldAgreeWithUnrecognizedOptions()
		{
			(int met, List<string> disagreements) = await Compare(1705,
				random => random.Next(4), new ObjectEqualityOptions<int>());

			await That(disagreements).IsEmpty();
			await That(met).IsGreaterThan(100);
		}

		[Test]
		public async Task NumbersComparedAsObjects_ShouldAgreeWithUnrecognizedOptions()
		{
			(int met, List<string> disagreements) = await Compare(1529,
				random => random.Next(4), new ObjectEqualityOptions<object?>());

			await That(disagreements).IsEmpty();
			await That(met).IsGreaterThan(100);
		}

		[Test]
		public async Task Strings_ShouldAgreeWithUnrecognizedOptions()
		{
			string?[] values = ["a", "b", "c", null,];

			(int met, List<string> disagreements) = await Compare<string?, string?>(1642,
				random => values[random.Next(values.Length)], new StringEqualityOptions("expected"));

			await That(disagreements).IsEmpty();
			await That(met).IsGreaterThan(100);
		}

		[Test]
		public async Task Strings_WhenCaseIsIgnored_ShouldMatchItemsThatDifferInCase()
		{
			StringEqualityOptions options = new("expected");
			options.IgnoringCase();

			string result = await Describe(["a", "B", null,], [null, "b", "A",], options);

			await That(result).IsEqualTo("False: ");
		}

		[Test]
		public async Task Strings_WhenTheFirstItemIsNotExpected_ShouldReportTheIndexOfEachItem()
		{
			StringEqualityOptions options = new("expected");

			string result = await Describe<string?, string?>(["a", "b",], ["x", "b", "y", "a",], options);

			await That(result).IsEqualTo("""
			                             True: it
			                               contained item "x" at index 0 that was not expected and
			                               contained item "y" at index 2 that was not expected
			                             """);
		}

		[Test]
		public async Task Strings_WhenTheLastItemIsNotExpected_ShouldReportItsIndex()
		{
			StringEqualityOptions options = new("expected");

			string result = await Describe<string?, string?>(["a", "b", "c",], ["c", "a", "x",], options);

			await That(result).IsEqualTo("""
			                             True: it
			                               contained item "x" at index 2 that was not expected and
			                               lacked 1 of 3 expected items: "b"
			                             """);
		}

		/// <summary>
		///     Describes random subjects for random expected items, of which every other one is the subject in a
		///     different order, with the <paramref name="options" /> and with the same options behind another type.
		/// </summary>
		private static async Task<(int Met, List<string> Disagreements)> Compare<T, T2>(int seed,
			Func<Random, T> createItem, IOptionsEquality<T2> options)
			where T : T2
		{
			Random random = new(seed);
			List<string> disagreements = new();
			int met = 0;
			for (int run = 0; run < 2000; run++)
			{
				T[] subject = Enumerable.Range(0, random.Next(0, 9)).Select(_ => createItem(random)).ToArray();
				T[] expected = random.Next(2) == 0
					? subject.OrderBy(_ => random.Next()).ToArray()
					: Enumerable.Range(0, random.Next(0, 6)).Select(_ => createItem(random)).ToArray();

				string recognized = await Describe(expected, subject, options);
				string unrecognized = await Describe(expected, subject, new ForwardingEquality<T2>(options));
				if (recognized == "False: ")
				{
					met++;
				}

				if (recognized != unrecognized)
				{
					disagreements.Add($"[{string.Join(",", subject)}] vs [{string.Join(",", expected)}]: " +
					                  $"{recognized} <> {unrecognized}");
				}
			}

			return (met, disagreements);
		}

		private static async Task<string> Describe<T, T2>(T[] expected, T[] subject, IOptionsEquality<T2> options)
			where T : T2
		{
			CollectionMatchOptions sut = new();
			sut.InAnyOrder();
			ICollectionMatcher<T, T2> matcher = sut.GetCollectionMatcher<T, T2>(expected);
			foreach (T item in subject)
			{
				(bool isFailure, string? error) = await matcher.Verify("it", item, options, 1);
				if (isFailure)
				{
					return $"failed early: {error}";
				}
			}

			(bool isCompleteFailure, string? completeError) = await matcher.VerifyComplete("it", options, 1);
			return $"{isCompleteFailure}: {completeError}";
		}

		private sealed class ForwardingEquality<T>(IOptionsEquality<T> options) : IOptionsEquality<T>
		{
			public ValueTask<bool> AreConsideredEqual<TExpected>(T actual, TExpected expected)
				=> options.AreConsideredEqual(actual, expected);
		}
	}

	public class DimensionsTests
	{
		[Test]
		[Arguments(false, false)]
		[Arguments(false, true)]
		[Arguments(true, false)]
		[Arguments(true, true)]
		public async Task ShouldNameAnUnexpectedItemByItsIndexInEveryDimension(bool inAnyOrder,
			bool ignoringDuplicates)
		{
			CollectionMatchOptions sut = new();
			if (inAnyOrder)
			{
				sut.InAnyOrder();
			}

			if (ignoringDuplicates)
			{
				sut.IgnoringDuplicates();
			}

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 4, 5,], [2, 3,]),
				[1, 2, 3, 4, 9, 5,]);

			await That(result).Contains("contained item 9 at index [1,1] that was not expected");
		}

		[Test]
		public async Task WhenAnItemIsIncorrect_ShouldNameItByItsIndexInEveryDimension()
		{
			CollectionMatchOptions sut = new();

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 0, 5, 6,], [2, 3,]),
				[1, 2, 3, 4, 5, 6,]);

			await That(result).IsEqualTo("True: it contained item 4 at index [1,0] instead of 0");
		}

		[Test]
		public async Task WhenAnItemIsInWrongOrder_ShouldNameItByItsIndexInEveryDimension()
		{
			CollectionMatchOptions sut = new();

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 4, 5, 3, 6,], [2, 3,]),
				[1, 2, 3, 4, 5, 6,]);

			await That(result).IsEqualTo("""
			                             True: it contained item 3 at index [0,2] in wrong order
			                             (but the items match in a different order)
			                             """);
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task WhenAnItemInterruptsTheExpectedItems_ShouldNameItByItsIndexInEveryDimension(
			bool ignoringDuplicates)
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringDuplicates(ignoringDuplicates);

			string result = await Describe(sut.GetCollectionMatcher<int, int>([3, 5,], [2, 3,]),
				[1, 2, 3, 4, 5, 6,]);

			await That(result).IsEqualTo("True: it contained item 4 at index [1,0] instead of 5");
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task WhenAnItemOfAContainedSubjectIsInWrongOrder_ShouldNameItByItsIndexInEveryDimension(
			bool ignoringDuplicates)
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
			sut.IgnoringDuplicates(ignoringDuplicates);

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 4, 5,], [2, 2,]),
				[2, 3, 4, 1,]);

			await That(result).IsEqualTo("True: it contained item 1 at index [1,1] in wrong order");
		}

		[Test]
		public async Task WhenThereAreTooManyDeviations_ShouldNameTheListedItemsByTheirIndexInEveryDimension()
		{
			CollectionMatchOptions sut = new();
			sut.InAnyOrder();

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 4, 5, 6,], [3, 2,]),
				[1, 7, 8, 9, 10, 11,]);

			await That(result).IsEqualTo("""
			                             failed early: it had more than 4 deviations:
			                               contained item 7 at index [0,1] that was not expected,
			                               contained item 8 at index [1,0] that was not expected,
			                               (… and maybe more)
			                             """);
		}

		[Test]
		public async Task WithMoreThanTwoDimensions_ShouldNameAnItemByItsIndexInEveryDimension()
		{
			CollectionMatchOptions sut = new();

			string result = await Describe(
				sut.GetCollectionMatcher<int, int>(Enumerable.Range(0, 24).Select(x => x == 17 ? -1 : x), [2, 3, 4,]),
				Enumerable.Range(0, 24));

			await That(result).IsEqualTo("True: it contained item 17 at index [1,1,1] instead of -1");
		}

		[Test]
		public async Task WithOneDimension_ShouldNameAnItemByItsPosition()
		{
			CollectionMatchOptions sut = new();

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 0,], [4,]), [1, 2, 3, 4,]);

			await That(result).IsEqualTo("True: it contained item 4 at index 3 instead of 0");
		}

		[Test]
		public async Task WithoutDimensions_ShouldNameAnItemByItsPosition()
		{
			CollectionMatchOptions sut = new();

			string result = await Describe(sut.GetCollectionMatcher<int, int>([1, 2, 3, 0,], null), [1, 2, 3, 4,]);

			await That(result).IsEqualTo("True: it contained item 4 at index 3 instead of 0");
		}

		private static async Task<string> Describe(ICollectionMatcher<int, int> matcher, IEnumerable<int> subject)
		{
			ObjectEqualityOptions<int> options = new();
			foreach (int item in subject)
			{
				(bool isFailure, string? error) = await matcher.Verify("it", item, options, 2);
				if (isFailure)
				{
					return $"failed early: {error}";
				}
			}

			(bool isCompleteFailure, string? completeError) = await matcher.VerifyComplete("it", options, 2);
			return $"{isCompleteFailure}: {completeError}";
		}
	}

	public class ExpectationItemTests
	{
		[Test]
		public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
		{
			void Act()
				=> _ = new CollectionMatchOptions.ExpectationItem<int>(null!, ExpectationGrammars.None,
					new EvaluationContext.EvaluationContext(), CancellationToken.None);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expectation").And
				.WithMessage("The 'expectation' cannot be null.").AsPrefix();
		}
	}

	public class FailureMessageTests
	{
		[Test]
		public async Task WhenAllOfManyExpectedItemsAreMissingInAnyOrder_ShouldCountThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(100, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenAllOfOneExpectedItemIsMissing_ShouldUseSingular()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3,]);

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenAllOfOneUniqueExpectedItemIsMissing_ShouldUseSingular()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3, 3,]).IgnoringDuplicates();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenAllOfTwoExpectedItemsAreMissing_ShouldUsePlural()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).Contains([3, 4,]);

			await That(Act).Throws<FailException>()
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

		[Test]
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

			await That(Act).Throws<FailException>()
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

		[Test]
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

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard().And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
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

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard().And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception))
				.Because("an item that the expectation did not answer fails the negation as well");
		}

		[Test]
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

		[Test]
		public async Task WhenIncorrectItemFormatsLikeTheExpectedItem_ShouldIncludeTheRuntimeType()
		{
			object[] subject = [0, 1,];
			object[] expected = [0, 1L,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).Equivalent();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenIncorrectItemIsALongStringThatDiffersAfterTheMaximumStringLength_ShouldShowTheDifference()
		{
			string common = new('a', 120);
			string[] subject = [common + "x",];
			string[] expected = [common + "y",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but it contained item "…aaaaaaaaaax" at index 0 instead of "…aaaaaaaaaay"
				             *
				             """).AsWildcard()
				.Because("both truncated texts would be identical, so the strings are shown from shortly before their first difference");
		}

		[Test]
		public async Task WhenManyExpectedItemsAreMissing_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenManyExpectedItemsAreMissingIgnoringDuplicates_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).IgnoringDuplicates();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenManyExpectedItemsAreMissingInAnyOrder_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenManyExpectedItemsAreMissingInAnyOrderIgnoringDuplicates_ShouldListTheFirstOfThem()
		{
			int[] subject = Enumerable.Range(1, 5).ToArray();
			int[] expected = Enumerable.Range(1, 30).ToArray();

			async Task Act()
				=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenPredicateThrowsInAnyOrder_AndTheItemIsAtAnotherPosition_ShouldNameTheIndexOfTheItem()
		{
			string[] subject = ["c", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard()
				.Because("the index names the subject item, not the position of the predicate that threw");
		}

		[Test]
		public async Task WhenPredicateThrowsInAnyOrder_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard()
				.Because("the item that the predicate did not answer is named like in the other collection expectations");
		}

		[Test]
		public async Task WhenPredicateThrowsInAnyOrderIgnoringDuplicates_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in any order ignoring duplicates,
				             but for the item at index 2, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard();
		}

		[Test]
		public async Task WhenPredicateThrowsInOrder_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == "b", x => x == "c" && Throw(x),] in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom
				             *
				             """).AsWildcard();
		}

		[Test]
		public async Task WhenPredicateThrowsInOrderIgnoringDuplicates_ShouldNameTheItemByItsIndex()
		{
			string[] subject = ["b", "b", "c",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == "b", x => x == "c" && Throw(x),]).IgnoringDuplicates();

			await That(Act).Throws<FailException>()
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

	public class GetCollectionMatcherTests
	{
		[Test]
		public async Task WhenAnExpectationIsNull_ShouldThrowArgumentException()
		{
			CollectionMatchOptions sut = new();
			CollectionMatchOptions.ExpectationItem<int>[] expected = [null!,];

			void Act()
				=> _ = sut.GetCollectionMatcher<int, int>(expected);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(false, true)]
		[Arguments(true, false)]
		[Arguments(true, true)]
		public async Task WhenAPredicateIsNull_ShouldThrowArgumentException(bool inAnyOrder, bool ignoringDuplicates)
		{
			CollectionMatchOptions sut = new();
			if (inAnyOrder)
			{
				sut.InAnyOrder();
			}

			if (ignoringDuplicates)
			{
				sut.IgnoringDuplicates();
			}

			Expression<Func<int, bool>>[] expected = [x => x > 0, null!,];

			void Act()
				=> _ = sut.GetCollectionMatcher<int, int>(expected);

			await That(Act).Throws<ArgumentException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
		}

		[Test]
		public async Task WhenAValueIsNull_ShouldNotThrow()
		{
			CollectionMatchOptions sut = new();
			string?[] expected = ["a", null,];

			void Act()
				=> _ = sut.GetCollectionMatcher<string?, string?>(expected);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenThePredicatesAreNotMaterialized_ShouldEnumerateThemOnce()
		{
			int enumerations = 0;

			IEnumerable<Expression<Func<int, bool>>> Expected()
			{
				enumerations++;
				yield return x => x > 0;
			}

			CollectionMatchOptions sut = new();

			ICollectionMatcher<int, int> matcher = sut.GetCollectionMatcher<int, int>(Expected());

			await That(matcher).IsNotNull();
			await That(enumerations).IsEqualTo(1);
		}
	}

	public class GetExpectationTests
	{
		[Test]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.None, "is equal to collection [1] in order")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural, "are equal to collection [1] in order")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Negated, "is not equal to collection [1] in order")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated, "are not equal to collection [1] in order")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.None, "contains collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural, "contain collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains,
			ExpectationGrammars.Plural | ExpectationGrammars.Negated,
			"do not contain collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.None, "is contained in collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			ExpectationGrammars.Plural, "are contained in collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
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

		[Test]
		[Arguments(false, false, false, "contains collection [1] in order and contiguous")]
		[Arguments(false, false, true, "contains collection [1] in order ignoring interspersed items")]
		[Arguments(false, true, false, "contains collection [1] in order and contiguous ignoring duplicates")]
		[Arguments(false, true, true, "contains collection [1] in order ignoring duplicates and interspersed items")]
		[Arguments(true, false, false, "contains collection [1] in any order")]
		[Arguments(true, true, false, "contains collection [1] in any order ignoring duplicates")]
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

		[Test]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent,
			"is equal to collection [1] in order")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains,
			"contains collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.ContainsProperly,
			"contains collection [1] and at least one additional item in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			"is contained in collection [1] in order and contiguous")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly,
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
		[Test]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateInAnyOrder()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["b", "a",]).InAnyOrder().IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Test]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateInSameOrder()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Test]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateWhenContained()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsContainedIn(["x", "a", "b", "y",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Test]
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

		[Test]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasing_ShouldBeADuplicateWhenContaining()
		{
			string[] subject = ["x", "a", "A", "b",];

			async Task Act()
				=> await That(subject).Contains(["x", "a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Test]
		public async Task WhenAnItemDiffersFromAnEarlierItemOnlyByCasingWithoutIgnoringCase_ShouldNotBeADuplicate()
		{
			string[] subject = ["a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["b", "a",]).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenAnItemMatchesAnExpectedItemAndTheNextOneInAnyOrder_ShouldStopReadingWhenContained()
		{
			int readItems = 0;

			IEnumerable<int> Source()
			{
				for (int i = 0; i < 1000; i++)
				{
					readItems++;
					yield return 5;
				}
			}

			async Task Act()
				=> await That(Source()).Contains([x => x > 0, x => x > 1,]).InAnyOrder().IgnoringDuplicates();

			await That(Act).DoesNotThrow();
			await That(readItems).IsEqualTo(1)
				.Because("the first item already matches both predicates, which decides the containment");
		}

		[Test]
		public async Task WhenAnItemMatchesExpectedItemsApartFromEachOtherInAnyOrder_ShouldStopReadingWhenContained()
		{
			int readItems = 0;

			IEnumerable<int> Source()
			{
				for (int i = 0; i < 10000; i++)
				{
					readItems++;
					yield return i % 2 == 0 ? 6 : 3;
				}
			}

			async Task Act()
				=> await That(Source()).Contains([x => x > 0, x => x == 3, x => x > 5,]).InAnyOrder()
					.IgnoringDuplicates();

			await That(Act).DoesNotThrow();
			await That(readItems).IsEqualTo(2)
				.Because("6 matches x > 0 and x > 5, and 3 matches x == 3");
		}

		[Test]
		public async Task WhenAnItemMatchesTwoExpectedValuesInAnyOrder_ShouldMatchBoth()
		{
			double[] subject = [1.1,];

			async Task Act()
				=> await That(subject).IsEqualTo([1.0, 1.2,]).Within(0.15).InAnyOrder().IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("each expected value is matched by an item, as in the same order");
		}

		[Test]
		public async Task WhenAnItemOnlyMatchesAnAlreadyMatchedExpectedValue_ShouldBeNoAdditionalItem()
		{
			string[] subject = ["a", "A",];

			async Task Act()
				=> await That(subject).Contains(["a",]).Properly().InAnyOrder().IgnoringDuplicates().IgnoringCase();

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenEqualExpectedItemsDifferForTheComparerInAnyOrder_ShouldNotBeDuplicates()
		{
			IdOnlyEquality[] subject = [new(1, "a"),];
			IdOnlyEquality[] expected = [new(1, "a"), new(1, "b"),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates()
					.Using(new ByNameComparer());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected * in any order ignoring duplicates,
				             but it lacked 1 of 2 expected items: *"b"*
				             """).AsWildcard()
				.Because("the comparer tells the expected items apart, although they are equal");
		}

		[Test]
		public async Task WhenEqualItemsDifferForTheComparerInAnyOrder_ShouldNotBeDuplicates()
		{
			IdOnlyEquality[] subject = [new(1, "a"), new(1, "b"),];
			IdOnlyEquality[] expected = [new(1, "a"),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates()
					.Using(new ByNameComparer());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected * in any order ignoring duplicates,
				             but it contained item *"b"* at index 1 that was not expected
				             *
				             """).AsWildcard()
				.Because("the comparer tells the items apart, although they are equal");
		}

		[Test]
		public async Task WhenEqualItemsDifferForTheComparerInSameOrder_ShouldNotBeDuplicates()
		{
			IdOnlyEquality[] subject = [new(1, "a"), new(1, "b"),];
			IdOnlyEquality[] expected = [new(1, "a"),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).IgnoringDuplicates().Using(new ByNameComparer());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected * in order ignoring duplicates,
				             but it contained item *"b"* at index 1 that was not expected
				             *
				             """).AsWildcard()
				.Because("the comparer tells the items apart, although they are equal");
		}

		[Test]
		public async Task WhenEqualObjectsDifferInTheirDateTimeKindInAnyOrder_ShouldNotBeDuplicates()
		{
			DateTime local = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Local);
			DateTime utc = DateTime.SpecifyKind(local, DateTimeKind.Utc);
			object[] subject = [local, utc,];
			object[] expected = [local,];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              is equal to collection expected in any order ignoring duplicates,
				              but it contained item {Formatter.Format(utc)} at index 1 that was not expected
				              *
				              """).AsWildcard()
				.Because("a Utc and a Local value with the same ticks denote different instants");
		}

		[Test]
		public async Task WhenExpectedValuesDifferOnlyByCasing_ShouldBeDuplicatesInAnyOrder()
		{
			string[] subject = ["a",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "A",]).IgnoringDuplicates().InAnyOrder().IgnoringCase();

			await That(Act).DoesNotThrow()
				.Because("\"a\" and \"A\" are duplicates when case is ignored");
		}

		[Test]
		public async Task WhenOneItemMatchesTwoPredicatesInSameOrder_ShouldSucceed()
		{
			int[] subject = [1,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x > 0, x => x == 1,]).IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("only which items occur matters, so the one item may match both predicates, as in any order");
		}

		[Test]
		public async Task WhenTheRunIsInterruptedInSameOrder_ShouldReportTheInterruptingItem()
		{
			int[] subject = [3, 0, 2, 2,];

			async Task Act()
				=> await That(subject).Contains([x => x >= 3, x => x == 2,]).IgnoringDuplicates();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [x => x >= 3, x => x == 2,] in order and contiguous ignoring duplicates,
				             but it contained item 0 at index 1 instead of x => (x == 2)

				             Collection:
				             [3, 0, 2, 2]

				             Expected:
				             [
				               x => (x >= 3),
				               x => (x == 2)
				             ]
				             """);
		}

		[Test]
		public async Task WhenTheRepeatedItemFitsAPredicateInSameOrder_ShouldChooseIt()
		{
			int[] subject = [3, 3, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x <= 3, x => x >= 0,]).IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("3 matches x <= 3 and 2 matches x >= 0");
		}

		[Test]
		public async Task WhenTwoItemsMatchTheSameExpectedValue_ShouldBeDuplicates()
		{
			string[] subject = ["abc", "axe",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a.*",]).AsRegex().IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("an item that matches an already matched expected value repeats it");
		}

		[Test]
		public async Task WhenTwoItemsMatchTheSamePredicate_ShouldBeDuplicates()
		{
			int[] subject = [1, 4, 2,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x > 0, x => x == 2,]).InAnyOrder().IgnoringDuplicates();

			await That(Act).DoesNotThrow()
				.Because("only which items occur matters, so 1 and 4 both count for x > 0, like for expected values");
		}

		[Test]
		[MethodDataSource(nameof(MatchedOnBothSidesCases))]
		public async Task WithAnyKindOfExpectedItems_ShouldOnlyRequireEachSideToBeMatched(
			CollectionMatchOptions.EquivalenceRelations relation, string mode, string kind, string subject,
			string expected, bool isMatch)
		{
			CollectionMatchOptions sut = new(relation);
			if (mode == "any")
			{
				sut.InAnyOrder();
			}
			else if (mode == "same-interspersed")
			{
				sut.IgnoringInterspersedItems();
			}

			sut.IgnoringDuplicates();
			IEnumerable<int> expectedValues = expected.Split(',').Select(int.Parse);
			ICollectionMatcher<int, int> matcher = kind switch
			{
				"predicates" => sut.GetCollectionMatcher<int, int>(expectedValues.Select(IsNear)),
				"expectations" => sut.GetCollectionMatcher<int, int>(expectedValues.Select(IsNearExpectation)),
				_ => sut.GetCollectionMatcher<int, int>(expectedValues),
			};

			bool result = await ReferenceCaseTests.Matches(matcher, subject.Split(',').Select(int.Parse),
				new ReferenceCaseTests.Equality("near"));

			await That(result).IsEqualTo(isMatch)
				.Because("only which items occur matters, not how often, for values, predicates and expectations alike");
		}

		public static IEnumerable<(CollectionMatchOptions.EquivalenceRelations, string, string, string, string, bool)>
			MatchedOnBothSidesCases()
		{
			const CollectionMatchOptions.EquivalenceRelations equivalent =
				CollectionMatchOptions.EquivalenceRelations.Equivalent;
			const CollectionMatchOptions.EquivalenceRelations contains =
				CollectionMatchOptions.EquivalenceRelations.Contains;
			const CollectionMatchOptions.EquivalenceRelations containsProperly =
				CollectionMatchOptions.EquivalenceRelations.ContainsProperly;
			const CollectionMatchOptions.EquivalenceRelations isContainedIn =
				CollectionMatchOptions.EquivalenceRelations.IsContainedIn;
			const CollectionMatchOptions.EquivalenceRelations isContainedInProperly =
				CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly;
			(string Subject, string Expected, string Modes, CollectionMatchOptions.EquivalenceRelations[] Matching)[]
				scenarios =
				[
					("5", "4,6", "any,same,same-interspersed", [equivalent, contains, isContainedIn,]),
					("5,0", "4,6", "any,same,same-interspersed", [contains, containsProperly,]),
					("5", "4,6,0", "any,same,same-interspersed", [isContainedIn, isContainedInProperly,]),
					("3,9", "3,9", "any,same,same-interspersed", [equivalent, contains, isContainedIn,]),
					("9,3", "3,9", "any", [equivalent, contains, isContainedIn,]),
					("9,3", "3,9", "same,same-interspersed", []),
				];
			List<(CollectionMatchOptions.EquivalenceRelations, string, string, string, string, bool)> data = [];
			foreach ((string subject, string expected, string modes,
				         CollectionMatchOptions.EquivalenceRelations[] matching) in scenarios)
			{
				foreach (CollectionMatchOptions.EquivalenceRelations relation in new[]
				         {
					         equivalent, contains, containsProperly, isContainedIn, isContainedInProperly,
				         })
				{
					foreach (string mode in modes.Split(','))
					{
						foreach (string kind in new[]
						         {
							         "values", "predicates", "expectations",
						         })
						{
							data.Add((relation, mode, kind, subject, expected, matching.Contains(relation)));
						}
					}
				}
			}

			return data;
		}

		private static Expression<Func<int, bool>> IsNear(int value)
			=> item => Math.Abs(item - value) <= 1;

		/// <remarks>
		///     Every expectation renders the same text, as it is built in a loop over the captured value.
		/// </remarks>
		private static CollectionMatchOptions.ExpectationItem<int> IsNearExpectation(int value)
			=> new(x => x.Satisfies(item => Math.Abs(item - value) <= 1), ExpectationGrammars.None,
				new EvaluationContext.EvaluationContext(), CancellationToken.None);

		private sealed class IdOnlyEquality(int id, string name)
		{
			public int Id { get; } = id;
			public string Name { get; } = name;

			public override bool Equals(object? obj) => obj is IdOnlyEquality other && other.Id == Id;

			public override int GetHashCode() => Id;
		}

		private sealed class ByNameComparer : IEqualityComparer<IdOnlyEquality>
		{
			public bool Equals(IdOnlyEquality? x, IdOnlyEquality? y) => x?.Name == y?.Name;

			public int GetHashCode(IdOnlyEquality obj) => obj.Name.GetHashCode();
		}
	}

	public class IgnoringInterspersedItemsTests
	{
		[Test]
		public async Task WhenInAnyOrderIsSpecified_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.InAnyOrder();

			void Act() => sut.IgnoringInterspersedItems();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringInterspersedItems cannot be combined with InAnyOrder.")
				.Because("the any-order match never requires contiguous items, so the option would silently be dropped");
		}

		[Test]
		public async Task WhenInAnyOrderIsSpecifiedAfterwards_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems();

			void Act() => sut.InAnyOrder();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("InAnyOrder cannot be combined with IgnoringInterspersedItems.")
				.Because("the any-order match never requires contiguous items, so the option would silently be dropped");
		}

		[Test]
		public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems();

			void Act() => sut.IgnoringInterspersedItems();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringInterspersedItems cannot be specified more than once.");
		}

		[Test]
		[Arguments(true, true)]
		[Arguments(true, false)]
		[Arguments(false, true)]
		[Arguments(false, false)]
		public async Task WhenSpecifiedTwiceWithAValue_ShouldThrowInvalidOperationException(bool first, bool second)
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems(first);

			void Act() => sut.IgnoringInterspersedItems(second);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringInterspersedItems cannot be specified more than once.")
				.Because("the second value would silently replace the first one");
		}

		[Test]
		public async Task WithFalse_ShouldDescribeTheItemsAsContiguous()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			string expected = sut.GetExpectation("[1]", ExpectationGrammars.None);

			sut.IgnoringInterspersedItems(false);

			await That(sut.GetExpectation("[1]", ExpectationGrammars.None)).IsEqualTo(expected);
		}

		[Test]
		public async Task WithFalse_WhenInAnyOrderIsSpecified_ShouldNotThrow()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.InAnyOrder();

			void Act() => sut.IgnoringInterspersedItems(false);

			await That(Act).DoesNotThrow()
				.Because("not ignoring interspersed items states the default, which does not compete with the order");
			await That(sut.GetExpectation("[1]", ExpectationGrammars.None))
				.IsEqualTo("contains collection [1] in any order");
		}

		[Test]
		public async Task WithFalse_WhenInAnyOrderIsSpecifiedAfterwards_ShouldNotThrow()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems(false);

			void Act() => sut.InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("not ignoring interspersed items states the default, which does not compete with the order");
			await That(sut.GetExpectation("[1]", ExpectationGrammars.None))
				.IsEqualTo("contains collection [1] in any order");
		}

		[Test]
		public async Task WithTrue_WhenInAnyOrderIsSpecified_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.InAnyOrder();

			void Act() => sut.IgnoringInterspersedItems(true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringInterspersedItems cannot be combined with InAnyOrder.");
		}

		[Test]
		public async Task WithTrue_WhenInAnyOrderIsSpecifiedAfterwards_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.IgnoringInterspersedItems(true);

			void Act() => sut.InAnyOrder();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("InAnyOrder cannot be combined with IgnoringInterspersedItems.");
		}
	}

	public class InWrongOrderTests
	{
		[Test]
		public async Task Contains_WhenADuplicateIsInOrder_ShouldReportTheInterruptingItem()
		{
			int[] subject = [2, 1, 9, 2,];

			async Task Act()
				=> await That(subject).Contains([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2,] in order and contiguous,
				             but it contained item 9 at index 2 instead of 2

				             Collection:
				             [2, 1, 9, 2]

				             Expected:
				             [1, 2]
				             """)
				.Because("the 2 at index 3 follows the 1, so only the 9 keeps the items from being contiguous");
		}

		[Test]
		public async Task Contains_WhenADuplicateIsInOrderBehindMoreItemsThanAreInWrongOrder_ShouldReportTheItemInWrongOrder()
		{
			int[] subject = [2, 1, 9, 9, 2,];

			async Task Act()
				=> await That(subject).Contains([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2,] in order and contiguous,
				             but it contained item 1 at index 1 in wrong order

				             Collection:
				             [2, 1, 9, 9, 2]

				             Expected:
				             [1, 2]
				             """)
				.Because("one item in the wrong order is the shorter explanation than two interrupting items");
		}

		[Test]
		public async Task Contains_WhenADuplicateIsInOrderIgnoringInterspersedItems_ShouldOnlyReportTheMissingItem()
		{
			int[] subject = [2, 1, 2,];

			async Task Act()
				=> await That(subject).Contains([1, 2, 3,]).IgnoringInterspersedItems();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2, 3,] in order ignoring interspersed items,
				             but it lacked 1 of 3 expected items: 3

				             Collection:
				             [2, 1, 2]

				             Expected:
				             [1, 2, 3]
				             """);
		}

		[Test]
		public async Task Contains_WhenDuplicatesOfSeveralItemsAreInOrder_ShouldReportTheInterruptingItem()
		{
			int[] subject = [1, 2, 0, 1, 9, 2,];

			async Task Act()
				=> await That(subject).Contains([0, 1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [0, 1, 2,] in order and contiguous,
				             but it contained item 9 at index 4 instead of 2

				             Collection:
				             [1, 2, 0, 1, 9, 2]

				             Expected:
				             [0, 1, 2]
				             """);
		}

		[Test]
		public async Task Contains_WhenNoDuplicateIsInOrder_ShouldReportTheItemInWrongOrder()
		{
			int[] subject = [1, 1, 2,];

			async Task Act()
				=> await That(subject).Contains([2, 1,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [2, 1,] in order and contiguous,
				             but it contained item 2 at index 2 in wrong order

				             Collection:
				             [1, 1, 2]

				             Expected:
				             [2, 1]
				             """);
		}

		[Test]
		public async Task Contains_WhenThePredicateThrowsForAnItemThatIsNotAssigned_ShouldNotMatchIt()
		{
			int[] subject = [7, 1, 3, 0, 7,];

			async Task Act()
				=> await That(subject).Contains([x => x == 1, x => 9 / x == 3, x => x == 7,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [x => x == 1, x => 9 / x == 3, x => x == 7,] in order and contiguous,
				             but it contained item 0 at index 3 instead of x => (x == 7)

				             Collection:
				             [7, 1, 3, 0, 7]

				             Expected:
				             [
				               x => (x == 1),
				               x => ((9 / x) == 3),
				               x => (x == 7)
				             ]
				             """)
				.Because("only the explanation compares the 0 with the predicate that throws for it");
		}

		[Test]
		public async Task Contains_WhenThePredicateThrowsForAnItemThatIsNotAssignedUnderNegation_ShouldSucceed()
		{
			int[] subject = [7, 1, 3, 0, 7,];

			async Task Act()
				=> await That(subject).DoesNotContain([x => x == 1, x => 9 / x == 3, x => x == 7,]);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_WithPredicates_WhenADuplicateIsInOrder_ShouldReportTheInterruptingItem()
		{
			int[] subject = [2, 1, 9, 2,];

			async Task Act()
				=> await That(subject).Contains([x => x == 1, x => x == 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [x => x == 1, x => x == 2,] in order and contiguous,
				             but it contained item 9 at index 2 instead of x => (x == 2)

				             Collection:
				             [2, 1, 9, 2]

				             Expected:
				             [
				               x => (x == 1),
				               x => (x == 2)
				             ]
				             """);
		}

		[Test]
		public async Task Contains_WhenAnotherDuplicateIsWithinTheRun_ShouldReportItInsteadOfTheNextExpectedItem()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([1, 2, 3,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2, 3,] in order and contiguous,
				             but it contained item 2 at index 2 instead of 3

				             Collection:
				             [1, 2, 2, 3]

				             Expected:
				             [1, 2, 3]
				             """)
				.Because("an interrupting item is not reported instead of an expected item that it is equal to");
		}

		[Test]
		public async Task Contains_WhenTheRunStartsAtADuplicate_ShouldReportTheItemsThatInterruptTheShortestRun()
		{
			int[] subject = [1, 1, 5, 2, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([1, 2, 3,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2, 3,] in order and contiguous,
				             but it
				               contained item 5 at index 2 instead of 2 and
				               contained item 2 at index 4 instead of 3

				             Collection:
				             [1, 1, 5, 2, 2, 3]

				             Expected:
				             [1, 2, 3]
				             """);
		}

		[Test]
		public async Task Contains_WithPredicates_WhenSeveralItemsOfTheRunMatchAPredicate_ShouldReportTheItemThatMatchesNone()
		{
			int[] subject = [1, 5, 0, 7,];

			async Task Act()
				=> await That(subject).Contains([x => x == 1, x => x == 0 || x == 5, x => x == 7,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [x => x == 1, x => x == 0 || x == 5, x => x == 7,] in order and contiguous,
				             but it contained item 0 at index 2 instead of x => (x == 7)

				             Collection:
				             [1, 5, 0, 7]

				             Expected:
				             [
				               x => (x == 1),
				               x => ((x == 0) OrElse (x == 5)),
				               x => (x == 7)
				             ]
				             """)
				.Because("the 5 matches the second predicate, so it is not reported instead of it");
		}

		[Test]
		public async Task IsContainedIn_WhenADuplicateIsInOrder_ShouldReportTheGapInTheExpectedItems()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).IsContainedIn([2, 1, 9, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [2, 1, 9, 2,] in order and contiguous,
				             but it contained item 2 at index 1 instead of 9

				             Collection:
				             [1, 2]

				             Expected:
				             [2, 1, 9, 2]
				             """);
		}

		[Test]
		public async Task
			IsContainedIn_WhenADuplicateIsInOrderIgnoringInterspersedItems_ShouldOnlyReportTheUnexpectedItem()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).IsContainedIn([2, 1, 2,]).IgnoringInterspersedItems();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [2, 1, 2,] in order ignoring interspersed items,
				             but it contained item 3 at index 2 that was not expected

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [2, 1, 2]
				             """);
		}

		[Test]
		public async Task IsContainedIn_WhenAnotherExpectedDuplicateIsWithinTheRun_ShouldReportTheNextItemInsteadOfIt()
		{
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).IsContainedIn([1, 2, 2, 3,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [1, 2, 2, 3,] in order and contiguous,
				             but it contained item 3 at index 2 instead of 2

				             Collection:
				             [1, 2, 3]

				             Expected:
				             [1, 2, 2, 3]
				             """)
				.Because("an item is not reported instead of an expected item that it is equal to");
		}

		[Test]
		public async Task IsEqualTo_WhenOnlyOneOfTwoMovedDuplicatesIsExpected_ShouldReportTheFirstOneInWrongOrder()
		{
			int[] subject = [1, 1, 7, 8, 9,];

			async Task Act()
				=> await That(subject).IsEqualTo([7, 8, 9, 1,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [7, 8, 9, 1,] in order,
				             but it
				               contained item 1 at index 0 in wrong order and
				               contained item 1 at index 1 that was not expected

				             Collection:
				             [1, 1, 7, 8, 9]

				             Expected:
				             [7, 8, 9, 1]
				             """);
		}

		[Test]
		public async Task IsEqualTo_WithPredicates_WhenAMovedItemMatchesSeveralPredicates_ShouldPairAsManyItemsAsPossible()
		{
			int[] subject = [1, 2, 7, 8, 9,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x == 7, x => x == 8, x => x == 9, x => x <= 2, x => x == 1,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => x == 7, x => x == 8, x => x == 9, x => x <= 2, x => x == 1,] in order,
				             but it
				               contained item 1 at index 0 in wrong order and
				               contained item 2 at index 1 in wrong order
				             (but the items match in a different order)

				             Collection:
				             [1, 2, 7, 8, 9]

				             Expected:
				             [
				               x => (x == 7),
				               x => (x == 8),
				               x => (x == 9),
				               x => (x <= 2),
				               x => (x == 1)
				             ]
				             """)
				.Because("the 1 takes the last predicate, so that the 2 is no unexpected item and no predicate is missing");
		}
	}

	public class OptionTests
	{
		[Test]
		public async Task IgnoringDuplicates_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new();
			sut.IgnoringDuplicates();

			void Act() => sut.IgnoringDuplicates();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringDuplicates cannot be specified more than once.");
		}

		[Test]
		[Arguments(true, true)]
		[Arguments(true, false)]
		[Arguments(false, true)]
		[Arguments(false, false)]
		public async Task IgnoringDuplicates_WhenSpecifiedTwiceWithAValue_ShouldThrowInvalidOperationException(
			bool first, bool second)
		{
			CollectionMatchOptions sut = new();
			sut.IgnoringDuplicates(first);

			void Act() => sut.IgnoringDuplicates(second);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("IgnoringDuplicates cannot be specified more than once.")
				.Because("the second value would silently replace the first one");
		}

		[Test]
		public async Task IgnoringDuplicates_WithFalse_ShouldNotMentionDuplicates()
		{
			CollectionMatchOptions sut = new();
			string expected = sut.GetExpectation("[1]", ExpectationGrammars.None);

			sut.IgnoringDuplicates(false);

			await That(sut.GetExpectation("[1]", ExpectationGrammars.None)).IsEqualTo(expected);
		}

		[Test]
		public async Task InAnyOrder_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new();
			sut.InAnyOrder();

			void Act() => sut.InAnyOrder();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("InAnyOrder cannot be specified more than once.");
		}

		[Test]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains,
			"contains collection [1] and at least one additional item")]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
			"is contained in collection [1] that has at least one additional item")]
		public async Task Properly_ShouldTurnTheRelationIntoItsProperForm(
			CollectionMatchOptions.EquivalenceRelations relation, string expectedPrefix)
		{
			CollectionMatchOptions sut = new(relation);

			sut.Properly();

			await That(sut.GetExpectation("[1]", ExpectationGrammars.None)).StartsWith(expectedPrefix);
		}

		[Test]
		public async Task Properly_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new(CollectionMatchOptions.EquivalenceRelations.Contains);
			sut.Properly();

			void Act() => sut.Properly();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Properly cannot be specified more than once.");
		}

		[Test]
		public async Task Properly_WhenTheRelationIsEquivalent_ShouldThrowInvalidOperationException()
		{
			CollectionMatchOptions sut = new();

			void Act() => sut.Properly();

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("Properly requires a containment relation, but the relation is Equivalent.");
		}
	}

	/// <summary>
	///     Cases in which the matchers deviated from a brute-force comparison over small collections.
	/// </summary>
	[Explicit]
	[Category(TestCategories.Slow)]
	public class ReferenceCaseTests
	{
		[Test]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent, "any", "near", "1,3,3,0", "2,3,0,1", true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "any", "near", "1,3,3,0", "2,3,0,1",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "any", "near", "2,0,0,0", "1,2",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent, "any", "pred", "0,2,1", ">=0,<=2,<=0",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "any", "pred", "1,2", ">=3,<=3,<=1",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "any-dup", "pred", "0,2,3,1", "<=3,==2,<=0",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "any-dup", "div2", "1,3,3,0", "2,3,0,1",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "any-dup", "near", "1,2", "1",
			false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly, "any-dup", "div2", "1,3,3,0",
			"2,3,0,1", false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same", "eq", "0,1", "1,0", false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-interspersed", "eq", "0,2,3,3",
			"2,3,0", false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "eq", "0,2,3,3", "2,3,0",
			false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "eq", "1,2,0,2", "1,1,0,0",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "same-dup", "eq", "2,0", "2,3,0,3",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "div2", "2,0,3,1", "1,0,2",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "div2", "2,0,3,1", "1,0,2",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.ContainsProperly, "same-dup", "div2", "0,2,1", "0,3",
			false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly, "same-dup", "div2", "0",
			"1,0", false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "pred", "0", "<=2,==0",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Equivalent, "same-dup", "pred", "3,3,2", "<=3,>=0",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "pred", "0,0,1,3",
			"==3,<=2,<=2", false)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup", "pred", "3,0,2,2", ">=1,==2",
			true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.Contains, "same-dup-interspersed", "pred", "2,0,3,1",
			"==0,>=2,>=3", true)]
		[Arguments(CollectionMatchOptions.EquivalenceRelations.IsContainedIn, "same-dup", "pred", "1,3,2,2",
			">=1,<=3,<=3", true)]
		public async Task ShouldAgreeWithTheReference(CollectionMatchOptions.EquivalenceRelations relation,
			string mode, string equality, string subject, string expected, bool isMatch)
		{
			CollectionMatchOptions sut = new(relation);
			if (mode is ['a', ..,])
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

		[Test]
		public async Task ComparisonsCompletingAsynchronously_ShouldAgreeWithSynchronousComparisons()
		{
			CollectionMatchOptions.EquivalenceRelations[] relations =
			[
				CollectionMatchOptions.EquivalenceRelations.Equivalent,
				CollectionMatchOptions.EquivalenceRelations.Contains,
				CollectionMatchOptions.EquivalenceRelations.ContainsProperly,
				CollectionMatchOptions.EquivalenceRelations.IsContainedIn,
				CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly,
			];
			string[] equalities = ["eq", "div2", "near",];
			Random random = new(1616);
			List<string> disagreements = new();
			for (int run = 0; run < 2000; run++)
			{
				CollectionMatchOptions.EquivalenceRelations relation = relations[random.Next(relations.Length)];
				bool isInAnyOrder = random.Next(2) == 0;
				bool isInterspersed = !isInAnyOrder &&
				                      relation != CollectionMatchOptions.EquivalenceRelations.Equivalent &&
				                      random.Next(2) == 0;
				string equality = equalities[random.Next(equalities.Length)];
				int[] subject = Enumerable.Range(0, random.Next(0, 7)).Select(_ => random.Next(4)).ToArray();
				int[] expected = Enumerable.Range(0, random.Next(1, 6)).Select(_ => random.Next(4)).ToArray();

				ICollectionMatcher<int, int> CreateMatcher()
				{
					CollectionMatchOptions sut = new(relation);
					if (isInAnyOrder)
					{
						sut.InAnyOrder();
					}

					if (isInterspersed)
					{
						sut.IgnoringInterspersedItems();
					}

					return sut.GetCollectionMatcher<int, int>(expected);
				}

				string synchronously = await Describe(CreateMatcher(), subject, new Equality(equality));
				string asynchronously = await Describe(CreateMatcher(), subject, new YieldingEquality(equality));
				if (synchronously != asynchronously)
				{
					disagreements.Add($"{relation} {(isInAnyOrder ? "in any order " : "")}" +
					                  $"{(isInterspersed ? "interspersed " : "")}{equality} " +
					                  $"[{string.Join(",", subject)}] vs [{string.Join(",", expected)}]: " +
					                  $"{synchronously} <> {asynchronously}");
				}
			}

			await That(disagreements).IsEmpty()
				.Because("the matchers continue where a comparison did not complete synchronously");
		}

		[Test]
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
					.Select(_ => (equality == "pred"
						? new[]
						{
							"==", ">=", "<=",
						}[random.Next(3)]
						: "") + random.Next(4))
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

		private static async Task<string> Describe(ICollectionMatcher<int, int> matcher, IEnumerable<int> subject,
			IOptionsEquality<int> options)
		{
			foreach (int item in subject)
			{
				(bool isFailure, string? error) = await matcher.Verify("it", item, options, 2);
				if (isFailure)
				{
					return $"failed early: {error}";
				}
			}

			(bool isCompleteFailure, string? completeError) = await matcher.VerifyComplete("it", options, 2);
			return $"{isCompleteFailure}: {completeError}";
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

			return isFound && relation switch
			{
				CollectionMatchOptions.EquivalenceRelations.ContainsProperly => subject.Length > expected.Length,
				CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly => subject.Length < expected.Length,
				_ => true,
			};
		}

		internal static async Task<bool> Matches(ICollectionMatcher<int, int> matcher, IEnumerable<int> subject,
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
		internal sealed class Equality(string kind) : IOptionsEquality<int>
		{
			public ValueTask<bool> AreConsideredEqual<TExpected>(int actual, TExpected expected)
				=> new(IsEqual(actual, expected));

			public bool IsEqual<TExpected>(int actual, TExpected expected)
				=> expected is int value && kind switch
				{
					"div2" => actual / 2 == value / 2,
					"near" => Math.Abs(actual - value) <= 1,
					_ => actual == value,
				};
		}

		/// <summary>
		///     Compares like <see cref="Equality" />, but every other comparison completes asynchronously.
		/// </summary>
		private sealed class YieldingEquality(string kind) : IOptionsEquality<int>
		{
			private readonly Equality _equality = new(kind);
			private int _comparisons;

			public ValueTask<bool> AreConsideredEqual<TExpected>(int actual, TExpected expected)
			{
				bool isEqual = _equality.IsEqual(actual, expected);
				return _comparisons++ % 2 == 0 ? new ValueTask<bool>(isEqual) : Yield(isEqual);
			}

			private static async ValueTask<bool> Yield(bool isEqual)
			{
				await Task.Yield();
				return isEqual;
			}
		}
	}

	public class RestartedMatchTests
	{
		[Test]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatch_ShouldBeContained()
		{
			int[] subject = [1, 2, 4, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatch_ShouldReportTheItemsBeforeTheMatchAsAdditional()
		{
			int[] subject = [1, 2, 5, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenTheMatchRestartsAfterAnInterruptedPartialMatchIgnoringDuplicates_ShouldBeContained()
		{
			int[] subject = [1, 2, 4, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]).IgnoringDuplicates();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldBeContained()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).Contains([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldNotBeContainedIn()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsContainedIn([2, 3,]);

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldNotBeEqual()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsNotEqualTo([2, 3,]);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenTheMatchRestartsAfterAPartialMatch_ShouldReportTheItemsBeforeTheMatchAsAdditional()
		{
			int[] subject = [1, 2, 2, 3,];

			async Task Act()
				=> await That(subject).IsEqualTo([2, 3,]);

			await That(Act).Throws<FailException>()
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

		[Test]
		public async Task
			WhenTheMatchWouldRestartAtARepeatedItemIgnoringDuplicates_ShouldSkipTheRepeatedItem()
		{
			string[] subject = ["x", "a", "A", "b",];

			async Task Act()
				=> await That(subject).IsEqualTo(["a", "b",]).IgnoringDuplicates().IgnoringCase();

			await That(Act).Throws<FailException>()
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
		[Test]
		public async Task Equivalent_ShouldNotHaveSubsetOrSupersetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.Equivalent;

			await That(subject).DoesNotHaveFlag(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
			await That(subject).DoesNotHaveFlag(CollectionMatchOptions.EquivalenceRelations.Contains);
		}

		[Test]
		public async Task ProperSubset_ShouldHaveSubsetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.IsContainedInProperly;

			await That(subject).HasFlag(CollectionMatchOptions.EquivalenceRelations.IsContainedIn);
		}

		[Test]
		public async Task ProperSuperset_ShouldHaveSupersetFlag()
		{
			CollectionMatchOptions.EquivalenceRelations subject
				= CollectionMatchOptions.EquivalenceRelations.ContainsProperly;

			await That(subject).HasFlag(CollectionMatchOptions.EquivalenceRelations.Contains);
		}
	}
}
