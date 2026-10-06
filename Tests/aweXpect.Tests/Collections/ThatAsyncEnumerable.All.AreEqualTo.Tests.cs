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
				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1)
							.And.All().AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task ShouldSupportNullableValues()
				{
					IAsyncEnumerable<int?> subject = Factory.GetConstantValueAsyncEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 5 for all items,
						             but none of at least 1 were

						             Not matching items:
						             [1, (… and maybe more)]

						             Collection:
						             [1, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_NegatedShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(42));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 not for all items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringItemTests
			{
				[Test]
				public async Task AsPrefix_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("#").AsPrefix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             starts with "#" for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [
						               "text",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsRegex_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("^#").AsRegex();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches regex "^#" for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [
						               "text",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsRegex_WhenExpectedIsAnEmptyPattern_ShouldThrowArgumentException()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("").AsRegexThroughOptions();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
						.WithParamName("expected");
				}

				[Test]
				public async Task AsSuffix_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("o").AsSuffix();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             ends with "o" for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               "# Title",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "# Title",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AsWildcard_WhenAnItemDoesNotMatchThePattern_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["# Title", "## Intro", "text",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("#*").AsWildcard();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             matches "#*" for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [
						               "text",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "# Title",
						               "## Intro",
						               "text",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}");

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-1");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-1" for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [
						               "item-2",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "item-1",
						               "item-1",
						               "item-2",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task ShouldSupportNullableValues()
				{
					IAsyncEnumerable<string?> subject = Factory.GetConstantValueAsyncEnumerable<string?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}", 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5").Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<string> subject = Factory.GetAsyncFibonacciNumbers(i => $"item-{i}", 10);

					async Task Act()
						=> await That(subject).All().AreEqualTo("item-5");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "item-5" for all items,
						             but none of at least 1 were

						             Not matching items:
						             [
						               "item-1",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "item-1",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenItemsDifferInCase_ShouldSucceedWhenIgnoringCase(bool ignoreCase)
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO",]);

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").IgnoringCase(ignoreCase);

					await That(Act).Throws<FailException>().OnlyIf(!ignoreCase)
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but only 1 of at least 2 were

						             Not matching items:
						             [
						               "FOO",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "foo",
						               "FOO",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					string constantValue = "foo";
					IAsyncEnumerable<string> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string constantValue = "foo";
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "foo" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemTests
			{
				[Test]
				public async Task WhenAllItemsMatch_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreEqualTo(1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 not for all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Test]
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
