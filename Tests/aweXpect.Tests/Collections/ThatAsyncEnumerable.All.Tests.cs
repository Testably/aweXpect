#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class Tests
		{
			[Test]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsLessThan(6)).WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             is less than 6 for all items,
					             but it could not be verified, because the evaluation was already canceled
					             """).AsPrefix();
			}

			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).All().Satisfy(_ => true).And
						.All().Satisfy(_ => true);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsEqualTo(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for all items,
					             but only 2 of at least 3 were
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsEqualTo(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for all items,
					             but only 4 of at least 5 were
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsEqualTo(0));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 1, 1, 1);

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsEqualTo(1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).All().ComplyWith(item => item.IsEqualTo(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for all items,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
