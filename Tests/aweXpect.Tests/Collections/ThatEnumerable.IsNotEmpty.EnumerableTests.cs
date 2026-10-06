using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEmpty
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).IsNotEmpty()
						.And.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayContainsValues_ShouldSucceed()
			{
				IEnumerable subject = new[] { "foo", };

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayIsEmpty_ShouldFail()
			{
				IEnumerable subject = Array.Empty<object>();

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsValues_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 1, 2,]);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = ToEnumerable((int[]) []);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableNegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsValues_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               2
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable((int[]) []);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEmpty());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was <null>
					             """);
			}
		}
	}
}
