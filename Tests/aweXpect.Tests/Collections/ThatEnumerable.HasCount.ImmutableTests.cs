#if NET8_0_OR_GREATER
using System.Collections;
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class ImmutableArrayTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("a count is never null");
			}

			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("a count is never null");
			}

			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayNotEqualToTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have exactly 3 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(4);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(2);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayNotBetweenTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotBetween(3).And(6);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have between 3 and 6 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).HasCount().NotBetween(3).And(6);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasCount().NotBetween(3).And(6));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has between 3 and 6 items,
					             but it had only 2 items

					             Collection:
					             [1, 2]
					             """);
			}
		}

		public sealed class ImmutableArrayNotGreaterThanTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotGreaterThan(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotGreaterThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have more than 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenNegatedAndImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasCount().NotGreaterThan(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has more than 3 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayNotGreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotGreaterThanOrEqualTo(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have at least 3 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotGreaterThanOrEqualTo(4);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasCount().NotGreaterThanOrEqualTo(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has at least 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayNotLessThanTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotLessThan(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotLessThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have fewer than 4 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenNegatedAndImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasCount().NotLessThan(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has fewer than 3 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayNotLessThanOrEqualToTests
		{
			[Test]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotLessThanOrEqualTo(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have at most 3 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotLessThanOrEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasCount().NotLessThanOrEqualTo(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has at most 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class EnumerableTests
		{
			[Test]
			public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsNotACollection_AndContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3, 4,]);

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had at least 3 items

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("a count is never null");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableEqualToTests
		{
			[Test]
			public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasCount().EqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had at least 1 item

					             Collection:
					             [1, (… and maybe more)]
					             """)
					.Because("a count is never null, so the enumeration can stop at the first item");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasCount().EqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableNotEqualToTests
		{
			[Test]
			public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have exactly 3 items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(4);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasCount().NotEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have exactly 2 items,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
