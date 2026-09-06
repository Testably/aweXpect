using System.Collections;
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

	/// <summary>
	///     The negated and the <see langword="null" /> subject forms of the expectations that select their verb.
	/// </summary>
	public sealed class NegatedTests
	{
		[Fact]
		public async Task AreAllUniqueForDictionary_ShouldUseSingularVerb()
		{
			Dictionary<int, string> subject = new() { [0] = "a", [1] = "b", };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has duplicate values,*").AsWildcard();
		}

		[Fact]
		public async Task AreAllUniqueForDictionaryWithMemberAccessor_ShouldUseSingularVerb()
		{
			Dictionary<int, string> subject = new() { [0] = "a", [1] = "bb", };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique(v => v.Length));

			await That(Act).Throws<XunitException>()
				.WithMessage("*has duplicate values for v => v.Length,*").AsWildcard();
		}

		[Fact]
		public async Task AreAllUniqueForReadOnlyDictionary_ShouldUseSingularVerb()
		{
			IReadOnlyDictionary<int, string> subject = new Dictionary<int, string> { [0] = "a", [1] = "b", };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique());

			await That(Act).Throws<XunitException>()
				.WithMessage("*has duplicate values,*").AsWildcard();
		}

		[Fact]
		public async Task AreAllUniqueForReadOnlyDictionaryWithMemberAccessor_ShouldUseSingularVerb()
		{
			IReadOnlyDictionary<int, string> subject = new Dictionary<int, string> { [0] = "a", [1] = "bb", };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique(v => v.Length));

			await That(Act).Throws<XunitException>()
				.WithMessage("*has duplicate values for v => v.Length,*").AsWildcard();
		}

		[Fact]
		public async Task ContainsForEnumerableWithNullSubject_ShouldUseSingularVerb()
		{
			IEnumerable? subject = null;

			async Task Act()
				=> await That(subject!).Contains((object?)1);

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it was <null>*").AsWildcard();
		}

		[Fact]
		public async Task HasItemThatWithNullSubject_ShouldUseSingularVerb()
		{
			IEnumerable<int>? subject = null;

			async Task Act()
				=> await That(subject!).HasItemThat(i => i.IsEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it was <null>*").AsWildcard();
		}

		[Fact]
		public async Task IsEmptyForEnumerable_ShouldUseSingularVerb()
		{
			IEnumerable subject = Array.Empty<int>();

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.IsEmpty());

			await That(Act).Throws<XunitException>()
				.WithMessage("*is not empty,*but it was*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemWithExpected_ShouldUseSingularVerb()
		{
			IEnumerable<int> subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemWithPredicate_ShouldUseSingularVerb()
		{
			IEnumerable<int> subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem(x => x == 1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemWithString_ShouldUseSingularVerb()
		{
			IEnumerable<string?> subject = ["a", "b",];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem("a"));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemForEnumerable_ShouldUseSingularVerb()
		{
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem((object?)1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemThat_ShouldUseSingularVerb()
		{
			IEnumerable<int> subject = [1, 2,];

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItemThat(i => i.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item that*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemWithPredicateForEnumerable_ShouldUseSingularVerb()
		{
			IEnumerable subject = new[] { 1, 2, };

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem(x => Equals(x, 1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     The same expectations on an <see cref="IAsyncEnumerable{T}" /> subject.
	/// </summary>
	public sealed class AsyncTests
	{
		[Fact]
		public async Task HasCountWithNullSubject_ShouldUseSingularVerb()
		{
			IAsyncEnumerable<int>? subject = null;

			async Task Act()
				=> await That(subject!).HasCount(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("*but it was <null>*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItem_ShouldUseSingularVerb()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemThat_ShouldUseSingularVerb()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItemThat(i => i.IsEqualTo(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item that*").AsWildcard();
		}

		[Fact]
		public async Task NotHasItemWithPredicate_ShouldUseSingularVerb()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasItem(x => x == 1));

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have item*").AsWildcard();
		}

		[Fact]
		public async Task NotHasSingle_ShouldUseSingularVerb()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1,]);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

			await That(Act).Throws<XunitException>()
				.WithMessage("*does not have a single item*").AsWildcard();
		}
	}
#endif
}
