using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed class NegatedQuantifier
{
	private static string Context(string? title, string items)
		=> title is null
			? ""
			: $"{Environment.NewLine}{Environment.NewLine}{title}:{Environment.NewLine}{Items(items)}";

	private static string Items(string items)
		=> $"[{Environment.NewLine}" +
		   string.Join($",{Environment.NewLine}", items.Split(',').Select(item => $"  \"{item}\"")) +
		   $"{Environment.NewLine}]";

	private static aweXpect.ThatEnumerable.Elements Quantify(IThat<IEnumerable<string>?> subject, string quantifier)
		=> quantifier switch
		{
			"All" => subject.All(),
			"None" => subject.None(),
			"AtLeast(1)" => subject.AtLeast(1),
			"AtLeast(2)" => subject.AtLeast(2),
			"AtMost(0)" => subject.AtMost(0),
			"AtMost(1)" => subject.AtMost(1),
			"Between(1, 2)" => subject.Between(1).And(2),
			"Exactly(1)" => subject.Exactly(1),
			"LessThan(1)" => subject.LessThan(1),
			"LessThan(2)" => subject.LessThan(2),
			"MoreThan(0)" => subject.MoreThan(0),
			"MoreThan(1)" => subject.MoreThan(1),
			_ => throw new ArgumentOutOfRangeException(nameof(quantifier)),
		};

	public sealed class Tests
	{
		[Fact]
		public async Task ComplyWith_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).ComplyWith(item => item.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but 1 of 2 were

				             Matching items:
				             [1]

				             Collection:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task ComplyWithForEnumerable_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).ComplyWith(item => item.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but 1 of 2 were

				             Matching items:
				             [1]

				             Collection:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task MoreThanAndAtMost_WhenBothFail_ShouldShowTheItemsOfBoth()
		{
			IEnumerable<string> subject = ["a", "b",];

			async Task Act()
				=> await That(subject).MoreThan(1).Satisfy(s => s == "a").And.AtMost(0).Satisfy(s => s == "a");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             satisfies s => s == "a" for more than one item and satisfies s => s == "a" for at most 0 items,
				             but only 1 of 2 did and 1 of 2 did

				             Not matching items:
				             [
				               "b"
				             ]

				             Matching items:
				             [
				               "a"
				             ]

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task NestedAreEqualTo_WhenNegated_ShouldNegateTheItemExpectationOnlyOnce()
		{
			string subject = "a\na";

			async Task Act()
				=> await That(subject).HasLines(l => l.DoesNotComplyWith(it => it.All().AreEqualTo("a")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines of which not all are equal to "a",
				             but all 2 were

				             Collection:
				             [
				               "a",
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task NestedAreUnique_WhenNegated_ShouldNegateTheItemExpectationOnlyOnce()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(l => l.DoesNotComplyWith(it => it.All().AreUnique()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines of which not all are unique,
				             but all 2 were

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Theory]
		[InlineData("All", "a,b", "all are", "only 1 of 2 were", "Not matching items", "b")]
		[InlineData("None", "a,a", "none are", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("AtLeast(1)", "b,b", "at least one is", "none of 2 were", null, "")]
		[InlineData("AtLeast(2)", "a,b", "at least 2 are", "only 1 of 2 were", "Not matching items", "b")]
		[InlineData("AtMost(0)", "a,a", "at most 0 are", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("AtMost(1)", "a,a", "at most one is", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("Between(1, 2)", "b,b", "between 1 and 2 are", "none of 2 were", null, "")]
		[InlineData("Exactly(1)", "a,a", "exactly one is", "2 of 2 were", null, "")]
		[InlineData("LessThan(1)", "a,a", "fewer than one is", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("LessThan(2)", "a,a", "fewer than 2 are", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("MoreThan(0)", "b,b", "more than 0 are", "none of 2 were", null, "")]
		[InlineData("MoreThan(1)", "a,b", "more than one is", "only 1 of 2 were", "Not matching items", "b")]
		public async Task NestedComplyWith_ShouldUseTheVerbNumberOfTheQuantifier(
			string quantifier, string lines, string expectedQuantifier, string expectedResult,
			string? expectedContextTitle, string expectedContextItems)
		{
			string subject = lines.Replace(',', '\n');

			async Task Act()
				=> await That(subject).HasLines(l => Quantify(l, quantifier).ComplyWith(s => s.IsEqualTo("a")));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has lines of which {expectedQuantifier} equal to "a",
				              but {expectedResult}{Context(expectedContextTitle, expectedContextItems)}

				              Collection:
				              {Items(lines)}
				              """);
		}

		[Theory]
		[InlineData("All", "a,a", "not all are", "all 2 were", null, "")]
		[InlineData("None", "b,b", "at least one is", "none of 2 were", null, "")]
		[InlineData("AtLeast(1)", "a,b", "none are", "1 of 2 were", "Matching items", "a")]
		[InlineData("AtLeast(2)", "a,a", "fewer than 2 are", "2 of 2 were", "Matching items", "a,a")]
		[InlineData("AtMost(0)", "b,b", "at least one is", "none of 2 were", null, "")]
		[InlineData("AtMost(1)", "a,b", "more than one is", "1 of 2 were", "Not matching items", "b")]
		[InlineData("Between(1, 2)", "a,b", "not between 1 and 2 are", "1 of 2 were", null, "")]
		[InlineData("Exactly(1)", "a,b", "not exactly one is", "1 of 2 were", null, "")]
		[InlineData("LessThan(1)", "b,b", "at least one is", "none of 2 were", null, "")]
		[InlineData("LessThan(2)", "a,b", "at least 2 are", "1 of 2 were", "Not matching items", "b")]
		[InlineData("MoreThan(0)", "a,b", "none are", "1 of 2 were", "Matching items", "a")]
		[InlineData("MoreThan(1)", "a,a", "at most one is", "2 of 2 were", "Matching items", "a,a")]
		public async Task NestedComplyWith_WhenNegated_ShouldUseTheVerbNumberOfTheComplement(
			string quantifier, string lines, string expectedQuantifier, string expectedResult,
			string? expectedContextTitle, string expectedContextItems)
		{
			string subject = lines.Replace(',', '\n');

			async Task Act()
				=> await That(subject).HasLines(l
					=> l.DoesNotComplyWith(it => Quantify(it, quantifier).ComplyWith(s => s.IsEqualTo("a"))));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has lines of which {expectedQuantifier} equal to "a",
				              but {expectedResult}{Context(expectedContextTitle, expectedContextItems)}

				              Collection:
				              {Items(lines)}
				              """);
		}

		[Fact]
		public async Task NestedNoneAreEqualTo_WhenNegated_ShouldNameTheComplementOfNone()
		{
			string subject = "b\nb";

			async Task Act()
				=> await That(subject).HasLines(l => l.DoesNotComplyWith(it => it.None().AreEqualTo("a")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines of which at least one is equal to "a",
				             but none of 2 were

				             Collection:
				             [
				               "b",
				               "b"
				             ]
				             """);
		}

		[Theory]
		[InlineData("All", "a,b", "all satisfy", "only 1 of 2 did", "Not matching items", "b")]
		[InlineData("None", "a,a", "none satisfy", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("AtLeast(1)", "b,b", "at least one satisfies", "none of 2 did", null, "")]
		[InlineData("AtLeast(2)", "a,b", "at least 2 satisfy", "only 1 of 2 did", "Not matching items", "b")]
		[InlineData("AtMost(0)", "a,a", "at most 0 satisfy", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("AtMost(1)", "a,a", "at most one satisfies", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("Between(1, 2)", "b,b", "between 1 and 2 satisfy", "none of 2 did", null, "")]
		[InlineData("Exactly(1)", "a,a", "exactly one satisfies", "2 of 2 did", null, "")]
		[InlineData("LessThan(1)", "a,a", "fewer than one satisfies", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("LessThan(2)", "a,a", "fewer than 2 satisfy", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("MoreThan(0)", "b,b", "more than 0 satisfy", "none of 2 did", null, "")]
		[InlineData("MoreThan(1)", "a,b", "more than one satisfies", "only 1 of 2 did", "Not matching items", "b")]
		public async Task NestedSatisfy_ShouldUseTheVerbNumberOfTheQuantifier(
			string quantifier, string lines, string expectedQuantifier, string expectedResult,
			string? expectedContextTitle, string expectedContextItems)
		{
			string subject = lines.Replace(',', '\n');

			async Task Act()
				=> await That(subject).HasLines(l => Quantify(l, quantifier).Satisfy(s => s == "a"));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has lines of which {expectedQuantifier} s => s == "a",
				              but {expectedResult}{Context(expectedContextTitle, expectedContextItems)}

				              Collection:
				              {Items(lines)}
				              """);
		}

		[Theory]
		[InlineData("All", "a,a", "not all satisfy", "all 2 did", null, "")]
		[InlineData("None", "b,b", "at least one satisfies", "none of 2 did", null, "")]
		[InlineData("AtLeast(1)", "a,b", "none satisfy", "1 of 2 did", "Matching items", "a")]
		[InlineData("AtLeast(2)", "a,a", "fewer than 2 satisfy", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("AtMost(0)", "b,b", "at least one satisfies", "none of 2 did", null, "")]
		[InlineData("AtMost(1)", "a,b", "more than one satisfies", "1 of 2 did", "Not matching items", "b")]
		[InlineData("Between(1, 2)", "a,b", "not between 1 and 2 satisfy", "1 of 2 did", null, "")]
		[InlineData("Exactly(1)", "a,b", "not exactly one satisfies", "1 of 2 did", null, "")]
		[InlineData("LessThan(1)", "b,b", "at least one satisfies", "none of 2 did", null, "")]
		[InlineData("LessThan(2)", "a,b", "at least 2 satisfy", "1 of 2 did", "Not matching items", "b")]
		[InlineData("MoreThan(0)", "a,b", "none satisfy", "1 of 2 did", "Matching items", "a")]
		[InlineData("MoreThan(1)", "a,a", "at most one satisfies", "2 of 2 did", "Matching items", "a,a")]
		public async Task NestedSatisfy_WhenNegated_ShouldNegateTheQuantifierOnceAndShowTheItemsThatExplainTheFailure(
			string quantifier, string lines, string expectedQuantifier, string expectedResult,
			string? expectedContextTitle, string expectedContextItems)
		{
			string subject = lines.Replace(',', '\n');

			async Task Act()
				=> await That(subject).HasLines(l
					=> l.DoesNotComplyWith(it => Quantify(it, quantifier).Satisfy(s => s == "a")));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              has lines of which {expectedQuantifier} s => s == "a",
				              but {expectedResult}{Context(expectedContextTitle, expectedContextItems)}

				              Collection:
				              {Items(lines)}
				              """);
		}

		[Fact]
		public async Task NotNestedAreEqualTo_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but 1 of 2 were

				             Matching items:
				             [1]

				             Collection:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task NotNestedAreEqualToForEnumerable_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but 1 of 2 were

				             Matching items:
				             [1]

				             Collection:
				             [1, 2]
				             """);
		}

		[Fact]
		public async Task NotNestedAreUnique_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			int[] subject = [1, 1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).AreUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is unique for no items,
				             but 1 of 3 were

				             Matching items:
				             [2]

				             Collection:
				             [1, 1, 2]
				             """);
		}

		[Theory]
		[InlineData("All", "a,a", "not for all items", "all 2 did", null, "")]
		[InlineData("None", "b,b", "for at least one item", "none of 2 did", null, "")]
		[InlineData("AtLeast(1)", "a,b", "for no items", "1 of 2 did", "Matching items", "a")]
		[InlineData("AtLeast(2)", "a,a", "for fewer than 2 items", "2 of 2 did", "Matching items", "a,a")]
		[InlineData("AtMost(0)", "b,b", "for at least one item", "none of 2 did", null, "")]
		[InlineData("AtMost(1)", "a,b", "for more than one item", "1 of 2 did", "Not matching items", "b")]
		[InlineData("Between(1, 2)", "a,b", "for not between 1 and 2 items", "1 of 2 did", null, "")]
		[InlineData("Exactly(1)", "a,b", "for not exactly one item", "1 of 2 did", null, "")]
		[InlineData("LessThan(1)", "b,b", "for at least one item", "none of 2 did", null, "")]
		[InlineData("LessThan(2)", "a,b", "for at least 2 items", "1 of 2 did", "Not matching items", "b")]
		[InlineData("MoreThan(0)", "a,b", "for no items", "1 of 2 did", "Matching items", "a")]
		[InlineData("MoreThan(1)", "a,a", "for at most one item", "2 of 2 did", "Matching items", "a,a")]
		public async Task NotNestedSatisfy_WhenNegated_ShouldShowTheItemsThatExplainTheFailure(
			string quantifier, string items, string expectedQuantifier, string expectedResult,
			string? expectedContextTitle, string expectedContextItems)
		{
			IEnumerable<string> subject = items.Split(',');

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => Quantify(it, quantifier).Satisfy(s => s == "a"));

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that subject
				              satisfies s => s == "a" {expectedQuantifier},
				              but {expectedResult}{Context(expectedContextTitle, expectedContextItems)}

				              Collection:
				              {Items(items)}
				              """);
		}

		[Fact]
		public async Task NotNestedSatisfyForEnumerable_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).Satisfy(item => Equals(item, 1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             satisfies item => Equals(item, 1) for no items,
				             but 1 of 2 did

				             Matching items:
				             [1]

				             Collection:
				             [1, 2]
				             """);
		}
	}

#if NET8_0_OR_GREATER
	public sealed class AsyncTests
	{
		[Fact]
		public async Task AreEqualTo_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).AreEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but at least 1 of at least 1 were

				             Matching items:
				             [1, (… and maybe more)]

				             Collection:
				             [1, (… and maybe more)]
				             """);
		}

		[Fact]
		public async Task AreUnique_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).AreUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is unique for no items,
				             but 1 of 3 were

				             Matching items:
				             [2]

				             Collection:
				             [1, 1, 2]
				             """);
		}

		[Fact]
		public async Task ComplyWith_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).ComplyWith(item => item.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but at least 1 of at least 1 were

				             Matching items:
				             [1, (… and maybe more)]

				             Collection:
				             [1, (… and maybe more)]
				             """);
		}

		[Fact]
		public async Task Satisfy_WhenNegatedAndSomeItemsMatch_ShouldShowTheMatchingItems()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.MoreThan(0).Satisfy(item => item == 1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             satisfies item => item == 1 for no items,
				             but at least 1 of at least 1 did

				             Matching items:
				             [1, (… and maybe more)]

				             Collection:
				             [1, (… and maybe more)]
				             """);
		}
	}
#endif
}
