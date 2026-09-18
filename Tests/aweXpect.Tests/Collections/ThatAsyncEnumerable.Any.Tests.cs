#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class Any
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but none of 3 were

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Any().ComplyWith(x => x.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for not at least one item,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at least one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "banana", "cherry");

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("apple", "cherry");

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "b" for at least one item,
					             but none of 2 were

					             Collection:
					             [
					               "apple",
					               "cherry"
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAnyItemComplies_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Any().ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not at least one item,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
#endif
