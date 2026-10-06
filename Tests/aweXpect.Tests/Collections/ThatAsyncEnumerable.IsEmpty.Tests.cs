#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class IsEmpty
	{
		public sealed class Tests
		{
			[Test]
			public async Task CancelledEnumerable_ShouldFail()
			{
				using CancellationTokenSource cts = new();
				await cts.CancelAsync();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).IsEmpty().WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it could not be verified, because the evaluation was already canceled
					             """);
			}

			[Test]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject =
					GetCancellingAsyncEnumerable(5, cts, CancellationToken.None);

				async Task Act()
					=> await That(subject).IsEmpty().WithCancellation(token);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               0,
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsValues_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 2);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSourceThrowsAfterTheFirstItem_ShouldListTheItem()
			{
				IAsyncEnumerable<int> subject = ThrowAfter(new InvalidOperationException("src"), 1, 2);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               (… and maybe more)
					             ]
					             """)
					.Because("the first item already decides the result, so the exception of the source must not replace it");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
