using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class EnumerablePredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => false)
						.And.DoesNotHaveItem(_ => false).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					0, 1, 2,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item _ => true at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					0, 1, 2,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item _ => true at index 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(42)
						.And.DoesNotHaveItem(42).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Insert(2, expected);
				IEnumerable subject = values;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(expected - 1).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail(
				List<int> values, int unexpected)
			{
				values.Add(0);
				values.Add(1);
				values.Insert(2, unexpected);
				IEnumerable subject = values;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item {unexpected} at index 2,
					              but it had item {unexpected} at index 2

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed(int unexpected)
			{
				IEnumerable subject = new[]
				{
					0, 1, unexpected,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(42).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 42 at index 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableEquivalentTests
		{
			[Fact]
			public async Task WhenEquivalentItemIsFound_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(3).Select(x => new MyClass(x));
				MyClass unexpected = new(2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent().AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item equivalent to MyClass {
					               StringValue = "",
					               Value = 2
					             } at index 2,
					             but it had item MyClass {
					               StringValue = "",
					               Value = 2
					             } at index 2

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEquivalentItemIsNotFound_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass unexpected = new(4);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableUsingTests
		{
			[Fact]
			public async Task WithAllDifferentComparer_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(4).Using(new AllEqualComparer()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item 4 using AllEqualComparer at index 1,
					             but it had item 2 at index 1

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}
	}
}
