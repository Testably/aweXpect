using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class BoxedDefaultImmutableArrayTests
	{
		[Test]
		public async Task AllAreEqualTo_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).All().AreEqualTo(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 1 for all items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Contains_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).Contains(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task ContainsCollection_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).Contains([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task DoesNotContain_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).DoesNotContain(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 1,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task EndsWith_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).EndsWith(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             ends with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task HasCount_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).HasCount(0);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 0 items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task HasSingle_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).HasSingle();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has a single item,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task ImmutableArraySubject_IsEmpty_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is also recognized where only IsEmpty and IsNotEmpty exist for it");
		}

		[Test]
		public async Task ImmutableArraySubject_IsNotEmpty_ShouldFail()
		{
			ImmutableArray<int> subject = default;

			async Task Act()
				=> await That(subject).IsNotEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is also recognized where only IsEmpty and IsNotEmpty exist for it");
		}

		[Test]
		public async Task IsContainedIn_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsContainedIn([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is contained in collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEmpty_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEmpty_WhenSubjectIsEmpty_ShouldSucceed()
		{
			IEnumerable<int> subject = ImmutableArray<int>.Empty;

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).DoesNotThrow()
				.Because("an empty ImmutableArray is an initialized collection");
		}

		[Test]
		public async Task IsEqualTo_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2,] in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsEqualTo_WhenExpectedIsDefault_ShouldSucceed()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);
			ImmutableArray<int> expected = default;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is equal to another one, like two null collections");
		}

		[Test]
		public async Task IsEqualTo_WhenExpectedIsNull_ShouldSucceed()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);
			IEnumerable<int>? expected = null;

			async Task Act()
				=> await That(subject).IsEqualTo(expected);

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is equal to null, like a null collection");
		}

		[Test]
		public async Task IsInAscendingOrder_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsInAscendingOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsNotContainedIn_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsNotContainedIn([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not contained in collection [1, 2,] in order and contiguous,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsNotEmpty_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsNotEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task IsNotEqualTo_ShouldSucceed()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsNotEqualTo([1, 2,]);

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is not equal to a collection, like a null collection");
		}

		[Test]
		public async Task IsNotEqualTo_WhenUnexpectedIsNull_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);
			IEnumerable<int>? unexpected = null;

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection unexpected in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is equal to null, like a null collection");
		}

		[Test]
		public async Task StartsWith_ShouldFail()
		{
			IEnumerable<int> subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).StartsWith(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             starts with [1],
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_Contains_ShouldFail()
		{
			IEnumerable subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).Contains(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_HasCount_ShouldFail()
		{
			IEnumerable subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).HasCount(0);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 0 items,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_IsEmpty_ShouldFail()
		{
			IEnumerable subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is empty,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_IsEqualTo_ShouldFail()
		{
			IEnumerable subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsEqualTo(new object[]
				{
					1, 2,
				});

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection new object[] { 1, 2, } in order,
				             but it was <null>
				             """)
				.Because("a default ImmutableArray is not initialized, like a null collection");
		}

		[Test]
		public async Task Untyped_IsNotEqualTo_ShouldSucceed()
		{
			IEnumerable subject = default(ImmutableArray<int>);

			async Task Act()
				=> await That(subject).IsNotEqualTo(new object[]
				{
					1, 2,
				});

			await That(Act).DoesNotThrow()
				.Because("a default ImmutableArray is not equal to a collection, like a null collection");
		}
	}
}
