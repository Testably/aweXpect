using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

/// <summary>
///     A subject that knows its number of items is read as it is, so its count and its enumerator are code of the
///     caller: an exception of them fails the expectation and its negation alike.
/// </summary>
public sealed class ThrowingCollectionSubject
{
	public sealed class CountTests
	{
		[Test]
		public async Task AllSatisfy_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).All().Satisfy(x => x < 2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 2 for all items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task Contains_WhenItIsNotContained_ShouldFailWithoutTheCollectionContext()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did not contain it
				             """);
		}

		[Test]
		public async Task HasCount_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task HasItem_WhenTheIndexIsCountedFromTheEnd_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).HasItem(3).AtIndexFromEnd(0);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 3 at index 0 from end,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEmpty_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsNotEmpty_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).IsNotEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task ReadOnlyCollection_HasCount_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingReadOnlyCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task UntypedReferenceTypeCollection_HasCount_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingReadOnlyCollection<string>(exception, ThrowingMembers.Count, "a", "b");

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task UntypedValueTypeCollection_HasCount_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingCollection<int>(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task Untyped_Contains_WhenOnlyTheCollectionContextReadsTheThrowingCount_ShouldFailWithoutIt()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, ThrowingMembers.Count, 1, 2)
			{
				CountReadsBeforeThrowing = 1,
			};

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did not contain it
				             """);
		}

		[Test]
		public async Task Untyped_HasCount_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).HasCount(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task Untyped_IsEmpty_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, ThrowingMembers.Count, 1, 2);

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}

	public sealed class DisposeTests
	{
		/// <remarks>
		///     Untyped items are cast while they are read, and the cast disposes the enumerator of the subject when it
		///     advances past the last item, so that the exception cannot be told apart from one of the enumeration.
		/// </remarks>
		[Test]
		public async Task Untyped_WhenDisposedAtTheEndOfTheItems_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, ThrowingMembers.Dispose, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenAdvancingThrowsAsWell_ShouldFailWithTheExceptionOfTheEnumeration()
		{
			InvalidOperationException exception = new("boom");
			ThrowingMembers members = ThrowingMembers.MoveNext | ThrowingMembers.Dispose;
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, members, 1, 2)
			{
				DisposeException = new NotSupportedException("dispose failed"),
			};

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task WhenAllItemsAreRead_ShouldIgnoreTheException()
		{
			InvalidOperationException exception = new("boom");
			ThrowingCollection<int> collection = new(exception, ThrowingMembers.Dispose, 1, 2);
			IEnumerable<int> subject = collection;

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2,]);

			await That(Act).DoesNotThrow();
			await That(collection.DisposeCount).IsEqualTo(1);
		}

		[Test]
		public async Task WhenTheEnumerationStopsEarly_ShouldIgnoreTheException()
		{
			InvalidOperationException exception = new("boom");
			ThrowingCollection<int> collection = new(exception, ThrowingMembers.Dispose, 1, 2);
			IEnumerable<int> subject = collection;

			async Task Act()
				=> await That(subject).Contains(1);

			await That(Act).DoesNotThrow();
			await That(collection.DisposeCount).IsEqualTo(1);
		}
	}

	public sealed class EnumerationTests
	{
		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task AllAreUnique_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).All().AreUnique();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is unique for all items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task AllComplyWith_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.IsLessThan(2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is less than 2 for all items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task AllSatisfy_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).All().Satisfy(x => x < 2);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x < 2 for all items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task Contains_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task DoesNotComplyWith_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.Contains(3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 3,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task EndsWith_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).EndsWith(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             ends with [3],
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task HasItemThat_WhenTheCurrentItemThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Current, 1, 2);

			async Task Act()
				=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).AtIndex(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to 3 at index 5,
				             but it did throw an InvalidOperationException:
				               boom

				             Collection:
				             [the enumeration did throw an InvalidOperationException: boom]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task HasItem_WhenTheEnumeratorThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2);

			async Task Act()
				=> await That(subject).HasItem(3).AtIndex(5);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 3 at index 5,
				             but it did throw an InvalidOperationException:
				               boom

				             Collection:
				             [the enumeration did throw an InvalidOperationException: boom]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task HasSingle_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1);

			async Task Act()
				=> await That(subject).HasSingle();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has a single item,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenAdvancingThrowsAfterTheLastItem_ShouldListTheItemsUpToTheException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.MoveNext, 1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([1, 3,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 3,] in order,
				             but it did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, (the enumeration did throw an InvalidOperationException: boom)]

				             Expected:
				             [1, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsEqualTo_WhenTheEnumeratorThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2);

			async Task Act()
				=> await That(subject).IsEqualTo([1, 3,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 3,] in order,
				             but it did throw an InvalidOperationException:
				               boom

				             Collection:
				             [the enumeration did throw an InvalidOperationException: boom]

				             Expected:
				             [1, 3]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task IsInAscendingOrder_WhenAdvancingThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.MoveNext, 1, 2);

			async Task Act()
				=> await That(subject).IsInAscendingOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it did throw an InvalidOperationException:
				               boom

				             Collection:
				             [1, 2, (the enumeration did throw an InvalidOperationException: boom)]
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task ReadOnlyCollection_Contains_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingReadOnlyCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task StartsWith_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).StartsWith(1, 2, 3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with [1, 2, 3],
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task UntypedGenericCollection_Contains_ShouldFailWithTheExceptionAsInnerException(
			ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingReadOnlyCollection<string>(exception, member, "a", "b");

			async Task Act()
				=> await That(subject).Contains("c");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to "c" at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task Untyped_AllComplyWith_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.Satisfies(y => y is int));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             satisfies y => y is int for all items,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		[Arguments(ThrowingMembers.Enumeration)]
		[Arguments(ThrowingMembers.MoveNext)]
		[Arguments(ThrowingMembers.Current)]
		public async Task Untyped_Contains_ShouldFailWithTheExceptionAsInnerException(ThrowingMembers member)
		{
			InvalidOperationException exception = new("boom");
			IEnumerable subject = new ThrowingUntypedCollection(exception, member, 1, 2);

			async Task Act()
				=> await That(subject).Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 3 at least once,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}

	public sealed class NestedTests
	{
		[Test]
		public async Task Eventually_WhenALaterAttemptDoesNotThrow_ShouldSucceed()
		{
			InvalidOperationException exception = new("boom");
			int calls = 0;
			Func<IEnumerable<int>> subject = () => calls++ == 0
				? new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2)
				: new List<int>
				{
					1,
					2,
				};

			async Task Act()
				=> await That(subject).Eventually().Within(5.Seconds()).CheckEvery(10.Milliseconds())
					.Contains(2);

			await That(Act).DoesNotThrow();
			await That(calls).IsEqualTo(2);
		}

		[Test]
		public async Task Eventually_WhenEveryAttemptThrows_ShouldFailWithTheExceptionAsInnerException()
		{
			InvalidOperationException exception = new("boom");
			Func<IEnumerable<int>> subject = ()
				=> new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2);

			async Task Act()
				=> await That(subject).Eventually().Within(50.Milliseconds()).CheckEvery(50.Milliseconds())
					.Contains(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             eventually contains an item equal to 3 at least once within 0:00.050,
				             but it did throw an InvalidOperationException:
				               boom
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Test]
		public async Task InThatAll_ShouldFailWithTheException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2);

			async Task Act()
				=> await ThatAll(
					That(subject).Contains(3),
					That(true).IsTrue());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains an item equal to 3 at least once
				              [02] Expected that true is True
				             but
				              [01] it did throw an InvalidOperationException:
				                     boom
				             """);
		}

		[Test]
		public async Task InThatAny_ShouldFailWithTheException()
		{
			InvalidOperationException exception = new("boom");
			IEnumerable<int> subject = new ThrowingCollection<int>(exception, ThrowingMembers.Enumeration, 1, 2);

			async Task Act()
				=> await ThatAny(
					That(subject).Contains(3),
					That(true).IsFalse());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that subject contains an item equal to 3 at least once
				              [02] Expected that true is False
				             but
				              [01] it did throw an InvalidOperationException:
				                     boom
				              [02] it was True
				             """);
		}
	}
}
