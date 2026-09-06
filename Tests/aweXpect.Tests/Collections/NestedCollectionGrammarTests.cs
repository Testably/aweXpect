using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     Verifies that collection expectations below a plural subject use plural verbs.
/// </summary>
/// <remarks>
///     <see href="https://github.com/Testably/aweXpect/issues/1027" /><br />
///     <c>HasLines</c> is used as the plural subject, because it gives access to the complete set of expectations
///     on <see cref="IEnumerable{T}" />. Only the verb is asserted here; the surrounding message is verified in the
///     tests of the individual expectations.
/// </remarks>
public sealed class NestedCollectionGrammar
{
	public sealed class Tests
	{
		[Fact]
		public async Task AreAllUnique_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\na").HasLines(lines => lines.AreAllUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which only have unique items,*").AsWildcard();
		}

		[Fact]
		public async Task AreAllUniqueWithMemberAccessor_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.AreAllUnique(l => l!.Length));

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which only have unique items for l => l!.Length,*").AsWildcard();
		}

		[Fact]
		public async Task DoesNotContain_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.DoesNotContain("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which do not contain "a",*""").AsWildcard();
		}

		[Fact]
		public async Task DoesNotEndWith_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.DoesNotEndWith("b"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which do not end with ["b"],*""").AsWildcard();
		}

		[Fact]
		public async Task DoesNotHaveCount_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.DoesNotHaveCount(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which do not have exactly 2 items,*").AsWildcard();
		}

		[Fact]
		public async Task DoesNotStartWith_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.DoesNotStartWith("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which do not start with ["a"],*""").AsWildcard();
		}

		[Fact]
		public async Task EndsWith_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.EndsWith("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which end with ["a"],*""").AsWildcard();
		}

		[Fact]
		public async Task HasItem_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.HasItem("c"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which have item equal to "c",*""").AsWildcard();
		}

		[Fact]
		public async Task HasItemThat_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.HasItemThat(i => i.IsEqualTo("c")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which have item that is equal to "c",*""").AsWildcard();
		}

		[Fact]
		public async Task HasSingle_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.HasSingle());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which have a single item,*").AsWildcard();
		}

		[Fact]
		public async Task IsContainedIn_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.IsContainedIn(["a",]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which are contained in collection ["a",] in order,*""").AsWildcard();
		}

		[Fact]
		public async Task IsEqualTo_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.IsEqualTo(["a", "c",]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which match collection ["a", "c",] in order,*""").AsWildcard();
		}

		[Fact]
		public async Task IsInAscendingOrder_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("b\na").HasLines(lines => lines.IsInAscendingOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which are in ascending order,*").AsWildcard();
		}

		[Fact]
		public async Task IsNotEmpty_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("").HasLines(lines => lines.IsNotEmpty());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which are not empty,*").AsWildcard();
		}

		[Fact]
		public async Task IsNotInAscendingOrder_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.IsNotInAscendingOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has lines which are not in ascending order,*").AsWildcard();
		}

		[Fact]
		public async Task StartsWith_ShouldUsePluralVerb()
		{
			async Task Act()
				=> await That("a\nb").HasLines(lines => lines.StartsWith("b"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""*has lines which start with ["b"],*""").AsWildcard();
		}

		[Fact]
		public async Task WhenSubjectIsNotNested_ShouldUseSingularVerb()
		{
			IEnumerable<string> subject = ["a", "a",];

			async Task Act()
				=> await That(subject).AreAllUnique();

			await That(Act).Throws<XunitException>()
				.WithMessage("*only has unique items,*").AsWildcard();
		}

		[Fact]
		public async Task WhenSubjectIsThePronoun_ShouldKeepSingularResultVerb()
		{
			async Task Act()
				=> await That("").HasLines(lines => lines.HasSingle());

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it was empty*").AsWildcard();
		}
	}
}
