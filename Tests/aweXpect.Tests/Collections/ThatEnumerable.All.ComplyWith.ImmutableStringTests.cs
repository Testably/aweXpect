#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class ComplyWith
		{
			public sealed class ImmutableStringTests
			{
				[Test]
				public async Task WhenAllItemsMatchExpectation_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExactlyOneItemMatchesExpectation_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["apple", "banana", "cherry",];

					async Task Act()
						=> await That(subject).Exactly(1).ComplyWith(it => it.StartsWith("b"));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					ImmutableArray<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenNotAllItemsMatch_ShouldFail()
				{
					ImmutableArray<string?> subject = ["apple", "banana", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" for all items,
						             but only 2 of 3 did

						             Not matching items:
						             [
						               "banana"
						             ]

						             Collection:
						             [
						               "apple",
						               "banana",
						               "avocado"
						             ]
						             """);
				}
			}

			public sealed class ImmutableStringNegatedTests
			{
				[Test]
				public async Task WhenAllItemsMatchExpectation_ShouldFail()
				{
					ImmutableArray<string?> subject = ["apple", "ant",];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.StartsWith("a")));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not start with "a" for all items,
						             but none of 2 did

						             Not matching items:
						             [
						               "apple",
						               "ant"
						             ]

						             Collection:
						             [
						               "apple",
						               "ant"
						             ]
						             """);
				}

				[Test]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					ImmutableArray<string?> subject = ["apple", "ant",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().ComplyWith(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}
			}
		}
	}
}
#endif
