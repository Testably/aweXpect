#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class IsNotEmpty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableContainsValues_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 2);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

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
			public async Task WhenSourceHangsAfterTheFirstItem_ShouldNotWaitForMoreItems()
			{
				IAsyncEnumerable<int> subject = HangAfter([1,]);

				async Task Act()
					=> await That(subject).IsNotEmpty().WithTimeout(30.Seconds());

				await That(Act).ExecutesIn().AtMost(10.Seconds())
					.Because("the first item already decides that the collection is not empty");
			}

			[Test]
			public async Task WhenSourceThrowsAfterTheFirstItem_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1);

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).DoesNotThrow()
					.Because("the first item already decides that the collection is not empty");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsNotEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
