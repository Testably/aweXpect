#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEmpty
	{
		public sealed class ImmutableTests
		{
			[Test]
			public async Task WhenArrayContainsValues_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["foo",];

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayIsEmpty_ShouldFail()
			{
				ImmutableArray<string> subject = [];

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
				ImmutableArray<int> subject = [1, 1, 2,];

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				ImmutableArray<int> subject = [];

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was empty
					             """);
			}
		}
	}
}
#endif
