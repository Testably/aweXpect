using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class LessThan
	{
		public sealed class ItemsTests
		{
			[Fact]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IEnumerable<int> subject = GetCancellingEnumerable(6, cts);

				async Task Act()
					=> await That(subject).LessThan(8).Satisfy(y => y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => y < 6 for fewer than 8 items,
					             but it could not be verified, because it was already canceled

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).LessThan(3).AreEqualTo(1)
						.And.LessThan(3).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).LessThan(2).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for fewer than 2 items,
					             but at least 2 of at least 2 were

					             Matching items:
					             [1, 1, (… and maybe more)]

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenArrayContainsSufficientlyFewEqualItems_ShouldSucceed()
			{
				int[] subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).LessThan(3).AreEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenArrayContainsTooManyEqualItems_ShouldFail()
			{
				int[] subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).LessThan(3).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for fewer than 3 items,
					             but 4 of 7 were

					             Matching items:
					             [1, 1, 1, 1]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSufficientlyFewEqualItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).LessThan(4).AreEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).LessThan(4).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for fewer than 4 items,
					             but at least 4 of at least 4 were

					             Matching items:
					             [1, 1, 1, 1, (… and maybe more)]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).LessThan(1).AreEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for fewer than one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "FOO", "bar",]);

				async Task Act()
					=> await That(subject).LessThan(2).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for fewer than 2 items,
					             but at least 2 of at least 2 were

					             Matching items:
					             [
					               "foo",
					               "FOO",
					               (… and maybe more)
					             ]

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).LessThan(2).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for fewer than 2 items,
					             but at least 2 of at least 2 were

					             Matching items:
					             [
					               "foo",
					               "foo",
					               (… and maybe more)
					             ]

					             Collection:
					             [
					               "foo",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).LessThan(1).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for fewer than one item,
					             but at least 1 of at least 1 were

					             Matching items:
					             [
					               "foo",
					               (… and maybe more)
					             ]

					             Collection:
					             [
					               "foo",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).LessThan(1).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for fewer than one item,
					             but it was <null>
					             """);
			}
		}
	}
}
