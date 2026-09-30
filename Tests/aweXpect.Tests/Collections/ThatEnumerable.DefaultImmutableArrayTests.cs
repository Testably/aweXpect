#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class DefaultImmutableArrayTests
	{
		[Fact]
		public async Task AllAreEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).All().AreEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task AllAreUnique_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).All().AreUnique();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is unique for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task AllComplyWith_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).All().ComplyWith(x => x.IsPositive());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is positive for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task AllSatisfy_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).All().Satisfy(x => x > 0);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             satisfies x => x > 0 for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task Contains_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).Contains(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task ContainsCollection_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).Contains([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task ContainsPredicate_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).Contains(x => x > 0);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item matching x => x > 0 at least once,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task ContainsString_ShouldFail()
		{
			ImmutableArray<string?> subject = default;

			async Task Act()
				=> await That(subject).Contains("a");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains "a" at least once,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotComplyWithIsEmpty_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotComplyWith(x => x.IsEmpty());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotContain_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotContain(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotContainCollection_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotContain([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotContainPredicate_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotContain(x => x > 0);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching x => x > 0,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotEndWith_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotEndWith(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not end with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotHaveItem_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotHaveItem(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotHaveItemThat_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotHaveItemThat(x => x.IsEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have an item that is equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task DoesNotStartWith_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).DoesNotStartWith(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not start with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task EndsWith_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).EndsWith(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             ends with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task HasCount_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).HasCount(0);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 0 items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task HasCountNotEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).HasCount().NotEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have exactly one item,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task HasItem_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).HasItem(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has an item equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task HasItemThat_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).HasItemThat(x => x.IsEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has an item that is equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task HasSingle_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).HasSingle();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a single item,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsContainedIn_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsContainedIn([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsEmpty_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2,] in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsInAscendingOrder_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsInAscendingOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsInAscendingOrderByMember_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsInAscendingOrder(x => x);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order by x => x,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsInDescendingOrder_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsInDescendingOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is in descending order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsNotContainedIn_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsNotContainedIn([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not contained in collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsNotEmpty_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsNotEmpty();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsNotEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsNotEqualTo([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection [1, 2,] in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task IsNotInAscendingOrder_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsNotInAscendingOrder();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not in ascending order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task NoneAreEqualTo_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).None().AreEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for no items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task StartsWith_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).StartsWith(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             starts with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Fact]
		public async Task StringAllAreEqualTo_ShouldFail()
		{
			ImmutableArray<string?> subject = default;

			async Task Act()
				=> await That(subject).All().AreEqualTo("a");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "a" for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}
	}
}
#endif
