using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItemThat
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotNull())
						.And.HasItemThat(it => it.IsNotNull()).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 1 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 3 at index 3,
					             but it did not contain any item at index 3

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsNullItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[] { "a", null, };

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotNull()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not null at index 1,
					             but it had item <null> at index 1

					             Collection:
					             [
					               "a",
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = new[] { 1, 2, 3, };

				async Task Act()
					=> await That(subject).HasItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithFromEnd_WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(1).FromEnd();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableNegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(1)).AtIndex(2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(2)).AtIndex(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsNotEqualTo(0)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is not equal to 0,
					             but it was <null>
					             """);
			}
		}
	}
}
