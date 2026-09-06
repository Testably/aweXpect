using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     Verifies that collection expectations below a plural subject use plural verbs.
/// </summary>
/// <remarks>
///     <see href="https://github.com/Testably/aweXpect/issues/1027" /><br />
///     <c>HasLines</c> is used as the plural subject, because it gives access to the complete set of expectations
///     on <see cref="IEnumerable{T}" />.
/// </remarks>
public sealed class NestedCollectionGrammar
{
	public sealed class Tests
	{
		[Fact]
		public async Task AreAllUnique_ShouldUsePluralVerb()
		{
			string subject = "a\na";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.AreAllUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which only have unique items,
				             but it contained 1 duplicate:
				               "a"

				             Collection:
				             [
				               "a",
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task AreAllUniqueWithMemberAccessor_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.AreAllUnique(l => l!.Length));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which only have unique items for l => l!.Length,
				             but it contained 1 duplicate:
				               1

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotContain_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.DoesNotContain("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which do not contain "a",
				             but it contained it at least once

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotEndWith_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.DoesNotEndWith("b"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which do not end with ["b"],
				             but it did end with [
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotHaveCount_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.DoesNotHaveCount(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which do not have exactly 2 items,
				             but it did

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotStartWith_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.DoesNotStartWith("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which do not start with ["a"],
				             but it did start with [
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task EndsWith_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.EndsWith("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which end with ["a"],
				             but it contained "b" at index 1 instead of "a"

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task HasItem_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.HasItem("c"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which have item equal to "c",
				             but it did not match at any index

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task HasItemThat_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.HasItemThat(i => i.IsEqualTo("c")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which have item that is equal to "c",
				             but it had item "b"

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task HasSingle_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.HasSingle());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which have a single item,
				             but it contained more than one item

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task HasSingleOnEmptyCollection_ShouldKeepSingularResultVerbForThePronoun()
		{
			string subject = "";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.HasSingle());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which have a single item,
				             but it was empty
				             """);
		}

		[Fact]
		public async Task IsContainedIn_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.IsContainedIn(["a",]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which are contained in collection ["a",] in order,
				             but it contained item "b" at index 1 that was not expected

				             Collection:
				             [
				               "a",
				               "b"
				             ]

				             Expected:
				             [
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task IsEqualTo_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.IsEqualTo(["a", "c",]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which match collection ["a", "c",] in order,
				             but it
				               contained item "b" at index 1 instead of "c" and
				               lacked 1 of 2 expected items: "c"

				             Collection:
				             [
				               "a",
				               "b"
				             ]

				             Expected:
				             [
				               "a",
				               "c"
				             ]
				             """);
		}

		[Fact]
		public async Task IsInAscendingOrder_ShouldUsePluralVerb()
		{
			string subject = "b\na";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.IsInAscendingOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which are in ascending order,
				             but it had "b" before "a" which is not in ascending order

				             Collection:
				             [
				               "b",
				               "a"
				             ]
				             """);
		}

		[Fact]
		public async Task IsNotEmpty_ShouldUsePluralVerb()
		{
			string subject = "";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.IsNotEmpty());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which are not empty,
				             but it was
				             """);
		}

		[Fact]
		public async Task IsNotInAscendingOrder_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.IsNotInAscendingOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which are not in ascending order,
				             but it was

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task StartsWith_ShouldUsePluralVerb()
		{
			string subject = "a\nb";

			async Task Act()
				=> await That(subject).HasLines(lines => lines.StartsWith("b"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has lines which start with ["b"],
				             but it contained "a" at index 0 instead of "b"

				             Collection:
				             [
				               "a",
				               "b"
				             ]
				             """);
		}

		[Fact]
		public async Task WhenNotNested_ShouldUseSingularVerb()
		{
			IEnumerable<string> subject = ["a", "a",];

			async Task Act()
				=> await That(subject).AreAllUnique();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             only has unique items,
				             but it contained 1 duplicate:
				               "a"

				             Collection:
				             [
				               "a",
				               "a"
				             ]
				             """);
		}
	}
}
