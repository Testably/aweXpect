#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class ImmutablePredicateTests
		{
			[Test]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have an item matching _ => true at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(3);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableItemTests
		{
			[Test]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have an item equal to 2 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).AtIndex(3);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableStringItemTests
		{
			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task ShouldSupportIgnoringCase(bool ignoreCase)
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("BAR").IgnoringCase(ignoreCase).AtIndex(1);

				await That(Act).Throws<FailException>().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "BAR" ignoring case at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("foo").AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("bar").AtIndex(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "bar" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]
					             """);
			}
		}

		public sealed class ImmutableEquivalentTests
		{
			[Test]
			public async Task WhenEquivalentItemIsFound_ShouldFail()
			{
				ImmutableArray<MyClass> subject =
				[
					..Factory.GetFibonacciNumbers(3).Select(x => new MyClass(x)),
				];
				MyClass unexpected = new(2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent().AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equivalent to MyClass {
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

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenEquivalentItemIsNotFound_ShouldSucceed()
			{
				ImmutableArray<MyClass> subject =
				[
					..Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)),
				];
				MyClass unexpected = new(4);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableUsingTests
		{
			[Test]
			public async Task WithAllDifferentComparer_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAllEqualComparer_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(4).Using(new AllEqualComparer()).AtIndex(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 4 using AllEqualComparer at index 1,
					             but it had item 2 at index 1

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}
	}
}
#endif
