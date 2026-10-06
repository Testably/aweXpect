using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

/// <summary>
///     An item for which the item expectations fail both ways (because code of the caller threw or the item is
///     <see langword="null" />) answers nothing, so it decides the collection expectation and its negation alike.
/// </summary>
public sealed class UnansweredItem
{
	private static bool Throw(int _, Exception exception)
		=> throw exception;

	private static bool IsB(string? value)
		=> value is null ? throw new InvalidOperationException("null") : value == "b";

	private static IEnumerable<int> Lazy(params int[] values)
	{
		foreach (int value in values)
		{
			yield return value;
		}
	}

	public sealed class Item(string? name)
	{
		public string? Name { get; } = name;
	}

	private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
	{
		public bool Equals(string? x, string? y)
			=> throw exception;

		public int GetHashCode(string obj)
			=> obj.GetHashCode();
	}

	public sealed class ComplyWithTests
	{
		[Test]
		public async Task All_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.Satisfies(y => y < 2 ? true : throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => y < 2 ? true : throw exception for all items,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task All_WhenItemExpectationWithContextsThrows_ShouldShowTheContextsOfTheItem()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["ABC", "DEF",];

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.IsEqualTo("abc").Using(new ThrowingComparer(exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "abc" using UnansweredItem.ThrowingComparer for all items,
				             but for the item at index 0, the comparer did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "ABC",
				               "DEF"
				             ]

				             Actual (item [0]):
				             ABC
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task All_WhenItemIsNull_ShouldFail()
		{
			string?[] subject = ["a", null,];

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with "a" for all items,
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "a",
				               <null>
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task All_WhenNegatedItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.DoesNotSatisfy(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not satisfy y => Throw(y, exception) for all items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task All_WhenNestedItemExpectationThrows_ShouldNameBothItems()
		{
			InvalidOperationException exception = new("boom");
			int[][] subject = [[1,], [2, 3,],];

			async Task Act()
				=> await That(subject).All()
					.ComplyWith(x => x.None().ComplyWith(y => y.Satisfies(z => z < 3 ? false : throw exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies z => z < 3 ? false : throw exception for no items for all items,
				             but for the item at index 1, for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               [
				                 1
				               ],
				               [
				                 2,
				                 3
				               ]
				             ]

				             Collection (item [1]):
				             [2, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task AtLeast_WhenThrowingItemFollowsTheDecidingItem_ShouldSucceed(bool isCountKnown)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = isCountKnown ? new[] { 1, 2, } : Lazy(1, 2);

			async Task Act()
				=> await That(subject).AtLeast(1).ComplyWith(x => x.Satisfies(y => y == 1 ? true : throw exception));

			await That(Act).DoesNotThrow()
				.Because("the first item already decides the outcome, whether the number of items is known or not");
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task AtLeast_WhenThrowingItemOfEnumerableFollowsTheDecidingItem_ShouldSucceed(bool isCountKnown)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = isCountKnown ? new[] { 1, 2, } : Lazy(1, 2);

			async Task Act()
				=> await That(subject).AtLeast(1).ComplyWith(x => x.Satisfies(y => y is 1 ? true : throw exception));

			await That(Act).DoesNotThrow()
				.Because("the first item already decides the outcome, whether the number of items is known or not");
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task AtMost_WhenThrowingItemFollowsTheDecidingItems_ShouldFailWithTheDecidingItems(
			bool isCountKnown)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = isCountKnown ? new[] { 1, 1, 2, } : Lazy(1, 1, 2);

			async Task Act()
				=> await That(subject).AtMost(1).ComplyWith(x => x.Satisfies(y => y == 1 ? true : throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => y == 1 ? true : throw exception for at most one item,
				             but at least 2 of at least 2 did

				             Matching items:
				             [1, 1, (… and maybe more)]

				             Collection:
				             *
				             """).AsWildcard().And
				.Whose(e => e.InnerException, i => i.IsNull())
				.Because("the first two items already decide the outcome, whether the number of items is known or not");
		}

		[Test]
		public async Task Dictionary_ValuesNone_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			Dictionary<string, int> subject = new()
			{
				["a"] = 1,
			};

			async Task Act()
				=> await That(subject).Values.None().ComplyWith(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has values of which none satisfy y => Throw(y, exception),
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection (values):
				             [1]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task Enumerable_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Satisfies(_ => throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies _ => throw exception for no items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task ImmutableArray_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			ImmutableArray<int> subject = [1, 2,];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => Throw(y, exception) for no items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task ImmutableStringArray_WhenItemIsNull_ShouldFail()
		{
			ImmutableArray<string?> subject = ["b", null,];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with "a" for no items,
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "b",
				               <null>
				             ]
				             """);
		}
#endif

		[Test]
		public async Task None_WhenCombinedItemExpectationThrows_ShouldNameTheItemBeforeItsResult()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.IsGreaterThan(5).Or.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is greater than 5 or satisfies y => Throw(y, exception) for no items,
				             but for the item at index 0, it was 1, which differs by -4 and the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task None_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => Throw(y, exception) for no items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task None_WhenItemIsNull_ShouldFail()
		{
			string?[] subject = ["b", null,];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with "a" for no items,
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "b",
				               <null>
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task None_WhenMemberOfItemIsNull_ShouldFail()
		{
			Item[] subject = [new("b"), new(null),];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Whose(i => i.Name, n => n.StartsWith("a")));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Name starts with "a" for no items,
				             but for the item at index 1, Name was <null>

				             Collection:
				             [
				               UnansweredItem.Item {
				                 Name = "b"
				               },
				               UnansweredItem.Item {
				                 Name = <null>
				               }
				             ]
				             """);
		}

		[Test]
		public async Task None_WhenOtherOperandDecidesEveryItem_ShouldSucceed()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).None()
					.ComplyWith(x => x.IsGreaterThan(5).And.Satisfies(y => Throw(y, exception)));

			await That(Act).DoesNotThrow()
				.Because("an item that is not greater than 5 does not comply, whatever the predicate answers");
		}

		[Test]
		[Arguments("AtLeast(1)", "for at least one item")]
		[Arguments("AtMost(1)", "for at most one item")]
		[Arguments("Between(1, 2)", "for between 1 and 2 items")]
		[Arguments("Exactly(1)", "for exactly one item")]
		[Arguments("LessThan(2)", "for fewer than 2 items")]
		[Arguments("MoreThan(0)", "for more than 0 items")]
		public async Task OtherQuantifiers_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException(
			string quantifier, string expectedQuantifier)
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await Quantify(That(subject), quantifier).ComplyWith(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage($"""
				              Expected that subject
				              satisfies y => Throw(y, exception) {expectedQuantifier},
				              but for the item at index 0, the predicate did throw an InvalidOperationException:
				                boom

				              Collection:
				              [1, 2]
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task Strings_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<string?> subject = ["a", "b",];

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Satisfies(_ => throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies _ => throw exception for no items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		private static aweXpect.ThatEnumerable.Elements<int> Quantify(IThat<IEnumerable<int>?> subject,
			string quantifier)
			=> quantifier switch
			{
				"AtLeast(1)" => subject.AtLeast(1),
				"AtMost(1)" => subject.AtMost(1),
				"Between(1, 2)" => subject.Between(1).And(2),
				"Exactly(1)" => subject.Exactly(1),
				"LessThan(2)" => subject.LessThan(2),
				"MoreThan(0)" => subject.MoreThan(0),
				_ => throw new ArgumentOutOfRangeException(nameof(quantifier)),
			};
	}

	public sealed class NegatedComplyWithTests
	{
		[Test]
		public async Task All_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject)
					.DoesNotComplyWith(it => it.All().ComplyWith(x => x.Satisfies(y => Throw(y, exception))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => Throw(y, exception) not for all items,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task HasItemThat_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject)
					.DoesNotComplyWith(it => it.HasItemThat(x => x.Satisfies(y => Throw(y, exception))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that satisfies y => Throw(y, exception),
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task None_WhenThrowingItemFollowsTheDecidingItem_ShouldSucceed(bool isCountKnown)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = isCountKnown ? new[] { 1, 2, } : Lazy(1, 2);

			async Task Act()
				=> await That(subject)
					.DoesNotComplyWith(it => it.None().ComplyWith(x => x.Satisfies(y => y == 1 ? true : throw exception)));

			await That(Act).DoesNotThrow()
				.Because("the first item already decides the outcome, whether the number of items is known or not");
		}
	}

	public sealed class HasItemThatTests
	{
		[Test]
		public async Task DoesNotHaveItemThat_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that satisfies y => Throw(y, exception),
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task DoesNotHaveItemThat_WhenItemIsNull_ShouldFail()
		{
			string?[] subject = ["b", null,];

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that starts with "a",
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "b",
				               <null>
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task Enumerable_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).HasItemThat(x => x.Satisfies(_ => throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that satisfies _ => throw exception,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task ImmutableArray_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			ImmutableArray<int> subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that satisfies y => Throw(y, exception),
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
#endif

		[Test]
		public async Task WhenItemAtIndexThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasItemThat(x => x.Satisfies(y => y != 2 ? true : throw exception)).AtIndex(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that satisfies y => y != 2 ? true : throw exception at index 1,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2, 3,];

			async Task Act()
				=> await That(subject).HasItemThat(x => x.Satisfies(y => y < 2 ? false : throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that satisfies y => y < 2 ? false : throw exception,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenItemExpectationWithContextsThrows_ShouldShowTheContextsOfTheItem()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["ABC", "DEF",];

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo("abc").Using(new ThrowingComparer(exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to "abc" using UnansweredItem.ThrowingComparer,
				             but for the item at index 0, the comparer did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "ABC",
				               "DEF"
				             ]

				             Actual (item [0]):
				             ABC
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenItemIsNull_ShouldFail()
		{
			string?[] subject = [null, "a",];

			async Task Act()
				=> await That(subject).HasItemThat(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that starts with "a",
				             but for the item at index 0, it was <null>

				             Collection:
				             [
				               <null>,
				               "a"
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task WhenMatchingItemPrecedesThrowingItem_ShouldSucceed()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).HasItemThat(x => x.Satisfies(y => y < 2 ? true : throw exception));

			await That(Act).DoesNotThrow();
		}
	}

	public sealed class ExpectationItemsInAnyOrderTests
	{
		[Test]
		public async Task IgnoringDuplicates_WhenItemFailsBothWaysForEveryExpectedItem_ShouldFail()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [2, 1, 2,];
			Action<IThat<int>>[] expected = [x => x.IsEqualTo(2), x => x.Satisfies(y => y == 2 ? false : throw exception),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in any order ignoring duplicates,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [2, 1, 2]

				             Expected:
				             [an item that is equal to 2, an item that satisfies y => y == 2 ? false : throw exception]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IgnoringDuplicates_WhenUnansweredItemMatchesAnotherExpectedItem_ShouldSucceed()
		{
			string?[] subject = [null, "a", null,];
			Action<IThat<string?>>[] expected = [x => x.StartsWith("a"), x => x.IsNull(),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Predicates_WhenItemFailsBothWaysForEveryExpectedItem_ShouldFail()
		{
			string?[] subject = ["b", null,];

			async Task Act()
				=> await That(subject).IsEqualTo([x => IsB(x), x => x == "c",]).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [x => IsB(x), x => x == "c",] in any order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               null

				             Collection:
				             [
				               "b",
				               <null>
				             ]

				             Expected:
				             [
				               x => IsB(x),
				               x => (x == "c")
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.Is<InvalidOperationException>());
		}

		[Test]
		public async Task Predicates_WhenThrowingItemMatchesAnotherExpectedItem_ShouldSucceed()
		{
			string?[] subject = [null, "b",];

			async Task Act()
				=> await That(subject).IsEqualTo([x => x!.Equals("b"), x => x == null,]).InAnyOrder();

			await That(Act).DoesNotThrow()
				.Because("the predicate that threw for null is only one of the candidates");
		}

		[Test]
		public async Task WhenItemFailsBothWaysForEveryExpectedItem_ShouldFail()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [2, 1,];
			Action<IThat<int>>[] expected = [x => x.IsEqualTo(2), x => x.Satisfies(y => Throw(y, exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in any order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [2, 1]

				             Expected:
				             [an item that is equal to 2, an item that satisfies y => Throw(y, exception)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenItemFailsBothWaysWithContexts_ShouldShowTheContextsOfTheItem()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["DEF", "abc", "GHI",];
			Action<IThat<string?>>[] expected =
				[x => x.IsEqualTo("abc"), x => x.IsEqualTo("def").Using(new ThrowingComparer(exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in any order,
				             but for the item at index 0, the comparer did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "DEF",
				               "abc",
				               "GHI"
				             ]

				             Expected:
				             [
				               an item that is equal to "abc",
				               an item that is equal to "def" using UnansweredItem.ThrowingComparer
				             ]

				             Actual (item [0]):
				             DEF
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenItemOnlyMatchesAnAssignedExpectedItem_ShouldBeANormalMismatch()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["a", "a",];
			Action<IThat<string?>>[] expected =
				[x => x.IsEqualTo("a"), x => x.Satisfies(s => s == "a" ? throw exception : false),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in any order,
				             but it
				               contained item "a" at index 1 that was not expected and
				               lacked 1 of 2 expected items: an item that satisfies s => s == "a" ? throw exception : false

				             Collection:
				             [
				               "a",
				               "a"
				             ]

				             Expected:
				             [
				               an item that is equal to "a",
				               an item that satisfies s => s == "a" ? throw exception : false
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task WhenUnansweredItemMatchesAnotherExpectedItem_ShouldSucceed()
		{
			string?[] subject = [null, "a",];
			Action<IThat<string?>>[] expected = [x => x.StartsWith("a"), x => x.IsNull(),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).InAnyOrder();

			await That(Act).DoesNotThrow();
		}
	}

	public sealed class ExpectationItemsTests
	{
		[Test]
		public async Task Contains_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.Satisfies(y => Throw(y, exception)),];

			async Task Act()
				=> await That(subject).Contains(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection expected in order and contiguous,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies y => Throw(y, exception)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task DoesNotContain_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.Satisfies(y => Throw(y, exception)),];

			async Task Act()
				=> await That(subject).DoesNotContain(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain collection expected in order and contiguous,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies y => Throw(y, exception)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsContainedIn_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.Satisfies(y => Throw(y, exception)), x => x.IsEqualTo(2),];

			async Task Act()
				=> await That(subject).IsContainedIn(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection expected in order and contiguous,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies y => Throw(y, exception), an item that is equal to 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.IsEqualTo(1), x => x.Satisfies(y => Throw(y, exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that is equal to 1, an item that satisfies y => Throw(y, exception)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenItemExpectationWithContextsThrows_ShouldShowTheContextsOfTheItem()
		{
			InvalidOperationException exception = new("boom");
			string[] subject = ["abc", "DEF",];
			Action<IThat<string?>>[] expected =
				[x => x.IsEqualTo("abc"), x => x.IsEqualTo("def").Using(new ThrowingComparer(exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the comparer did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "abc",
				               "DEF"
				             ]

				             Expected:
				             [
				               an item that is equal to "abc",
				               an item that is equal to "def" using UnansweredItem.ThrowingComparer
				             ]

				             Actual (item [1]):
				             DEF
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenItemIsNull_ShouldFail()
		{
			string?[] subject = ["a", null,];
			Action<IThat<string?>>[] expected = [x => x.StartsWith("a"), x => x.StartsWith("b"),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "a",
				               <null>
				             ]

				             Expected:
				             [
				               an item that starts with "a",
				               an item that starts with "b"
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task IsNotContainedIn_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.Satisfies(y => Throw(y, exception)), x => x.IsEqualTo(2),];

			async Task Act()
				=> await That(subject).IsNotContainedIn(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not contained in collection expected in order and contiguous,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies y => Throw(y, exception), an item that is equal to 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsNotEqualTo_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			int[] subject = [1, 2,];
			Action<IThat<int>>[] expected = [x => x.Satisfies(y => Throw(y, exception)), x => x.IsEqualTo(2),];

			async Task Act()
				=> await That(subject).IsNotEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection expected in order,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2]

				             Expected:
				             [an item that satisfies y => Throw(y, exception), an item that is equal to 2]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}

#if NET8_0_OR_GREATER
	public sealed class AsyncEnumerableTests
	{
		[Test]
		public async Task AllComplyWith_WhenItemIsNull_ShouldFail()
		{
			IAsyncEnumerable<string?> subject = ThatAsyncEnumerable.ToAsyncEnumerable<string?>("a", null);

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.StartsWith("a"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with "a" for all items,
				             but for the item at index 1, it was <null>

				             Collection:
				             [
				               "a",
				               <null>,
				               (… and maybe more)
				             ]
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task AtLeastComplyWith_WhenThrowingItemFollowsTheDecidingItem_ShouldSucceed()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2);

			async Task Act()
				=> await That(subject).AtLeast(1).ComplyWith(x => x.Satisfies(y => y == 1 ? true : throw exception));

			await That(Act).DoesNotThrow()
				.Because("the first item already decides the outcome, like for a synchronous collection");
		}

		[Test]
		public async Task DoesNotHaveItemThat_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2);

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(x => x.Satisfies(y => Throw(y, exception)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that satisfies y => Throw(y, exception),
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, (… and maybe more)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task HasItemThat_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2, 3);

			async Task Act()
				=> await That(subject).HasItemThat(x => x.Satisfies(y => y < 2 ? false : throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that satisfies y => y < 2 ? false : throw exception,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, (… and maybe more)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2);
			Action<IThat<int>>[] expected = [x => x.IsEqualTo(1), x => x.Satisfies(y => Throw(y, exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, (… and maybe more)]

				             Expected:
				             [an item that is equal to 1, an item that satisfies y => Throw(y, exception)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenItemExpectationWithContextsThrows_ShouldShowTheContextsOfTheItem()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<string> subject = ThatAsyncEnumerable.ToAsyncEnumerable("abc", "DEF");
			Action<IThat<string?>>[] expected =
				[x => x.IsEqualTo("abc"), x => x.IsEqualTo("def").Using(new ThrowingComparer(exception)),];

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected in order,
				             but for the item at index 1, the comparer did throw an InvalidOperationException:
				               boom

				             Collection:
				             [
				               "abc",
				               "DEF",
				               (… and maybe more)
				             ]

				             Expected:
				             [
				               an item that is equal to "abc",
				               an item that is equal to "def" using UnansweredItem.ThrowingComparer
				             ]

				             Actual (item [1]):
				             DEF
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task NoneComplyWith_WhenItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2, 3);

			async Task Act()
				=> await That(subject).None().ComplyWith(x => x.Satisfies(y => y < 2 ? false : throw exception));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => y < 2 ? false : throw exception for no items,
				             but for the item at index 1, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, (… and maybe more)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task NoneComplyWith_WhenNegatedAndItemExpectationThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable(1, 2);

			async Task Act()
				=> await That(subject)
					.DoesNotComplyWith(it => it.None().ComplyWith(x => x.Satisfies(y => Throw(y, exception))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => Throw(y, exception) for at least one item,
				             but for the item at index 0, the predicate did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, (… and maybe more)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}
#endif
}
