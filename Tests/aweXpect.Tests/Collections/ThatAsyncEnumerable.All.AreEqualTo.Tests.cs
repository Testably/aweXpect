#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class ItemTests
			{
				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1)
							.And.All().AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 11 were

						             Not matching items:
						             [2, 3, 5, 8, 13, 21, 34, 55, 89, (… and maybe more)]

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
				public async Task ShouldSupportNullableValues()
				{
					IAsyncEnumerable<int?> subject = Factory.GetConstantValueAsyncEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 5 for all items,
						             but only 1 of at least 11 were

						             Not matching items:
						             [
						               1,
						               1,
						               2,
						               3,
						               8,
						               13,
						               21,
						               34,
						               55,
						               89,
						               (… and maybe more)
						             ]

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
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_NegatedShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(42));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 not for all items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringItemTests
			{
				[Fact]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}");

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-1");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-1" for all items,
						             but only 2 of at least 11 were

						             Not matching items:
						             [
						               "item-2",
						               "item-3",
						               "item-5",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55",
						               "item-89",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               "item-3",
						               "item-5",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55",
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					IAsyncEnumerable<string?> subject = Factory.GetConstantValueAsyncEnumerable<string?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}", 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5").Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}", 10);

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-5" for all items,
						             but only 1 of 10 were

						             Not matching items:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               "item-3",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55"
						             ]

						             Collection:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               "item-3",
						               "item-5",
						               "item-8",
						               "item-13",
						               "item-21",
						               "item-34",
						               "item-55"
						             ]
						             """);
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenItemsDifferInCase_ShouldSucceedWhenIgnoringCase(bool ignoreCase)
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase(ignoreCase);

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreCase)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               "FOO"
						             ]

						             Collection:
						             [
						               "foo",
						               "FOO"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					string constantValue = "foo";
					IAsyncEnumerable<string> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string constantValue = "foo";
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemTests
			{
				[Fact]
				public async Task WhenAllItemsMatch_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 not for all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenOneItemDoesNotMatch_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(1));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
