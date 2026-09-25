using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class ComplyWith
		{
			public sealed class StringTests
			{
				[Fact]
				public async Task WhenAllItemsMatchExpectation_ShouldSucceed()
				{
					IEnumerable<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExactlyOneItemMatchesExpectation_ShouldSucceed()
				{
					IEnumerable<string?> subject = ["apple", "banana", "cherry",];

					async Task Act()
						=> await That(subject).Exactly(1).ComplyWith(it => it.StartsWith("b"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenExpectationsIsNull_WhenNegated_ShouldThrowArgumentNullException()
				{
					IEnumerable<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().ComplyWith(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenNotAllItemsMatch_ShouldFail()
				{
					IEnumerable<string?> subject = ["apple", "banana", "avocado",];

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" for all items,
						             but only 2 of 3 did
						             """).AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string?>? subject = null;

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.StartsWith("a"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" for all items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenAllItemsComplyUnderNegation_ShouldFail()
				{
					IEnumerable<string?> subject = ["apple", "ant", "avocado",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.All().ComplyWith(x => x.StartsWith("a")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "a" not for all items,
						             but all 3 did

						             Collection:
						             [
						               "apple",
						               "ant",
						               "avocado"
						             ]
						             """);
				}
			}
		}
	}
}
