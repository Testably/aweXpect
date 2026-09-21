#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class ImmutablePredicateTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

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
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(3);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableItemTests
		{
			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item 2 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("foo").AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("bar").AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item "bar" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringCase(bool ignoreCase)
			{
				ImmutableArray<string?> subject = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotHaveItem("BAR").IgnoringCase(ignoreCase).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not have item "BAR" ignoring case at index 1,
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
			[Fact]
			public async Task WhenEquivalentItemIsFound_ShouldFail()
			{
				ImmutableArray<MyClass> subject =
				[
					..Factory.GetFibonacciNumbers(3).Select(x => new MyClass(x)),
				];
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
			[Fact]
			public async Task WithAllDifferentComparer_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

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
#endif
