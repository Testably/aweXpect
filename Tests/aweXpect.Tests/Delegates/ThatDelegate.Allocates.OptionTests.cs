#if NET8_0_OR_GREATER
using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Allocates
	{
		public sealed class AfterWarmUpTests
		{
			[Test]
			public async Task WhenDelegateAllocatesOnEveryCall_ShouldFail()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().AtMost(100);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates at most 100 bytes after a warm-up,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocatesOnlyOnTheFirstCall_ShouldSucceed()
			{
				byte[]? cache = null;
				Action @delegate = () => cache ??= new byte[1000];

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().Nothing();

				await That(Act).DoesNotThrow()
					.Because("the one-time allocation happens in the warm-up, which is not measured");
			}

			[Test]
			public async Task WhenDelegateWithValueAllocatesOnlyOnTheFirstCall_ShouldSucceed()
			{
				byte[]? cache = null;
				Func<byte[]> @delegate = () => cache ??= new byte[1000];

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().Nothing();

				await That(Act).DoesNotThrow()
					.Because("the one-time allocation happens in the warm-up, which is not measured");
			}

			[Test]
			public async Task WhenSpecified_ShouldInvokeTheDelegateTwice()
			{
				int calls = 0;
				Func<int> @delegate = () => ++calls;

				await That(@delegate).Allocates().AfterWarmUp().Nothing();

				await That(calls).IsEqualTo(2)
					.Because("the warm-up invokes the delegate once before the measured invocation");
			}

			[Test]
			public async Task WhenSpecifiedTwice_ShouldWarmUpOnlyOnce()
			{
				int calls = 0;
				Func<int> @delegate = () => ++calls;

				await That(@delegate).Allocates().AfterWarmUp().AfterWarmUp().Nothing();

				await That(calls).IsEqualTo(2);
			}

			[Test]
			public async Task WhenTokenIsCanceledAndWarmUpThrows_ShouldNotInvokeTheDelegateAgain()
			{
				int calls = 0;
				using CancellationTokenSource cts = new();
				Action<CancellationToken> @delegate = token =>
				{
					calls++;
					cts.Cancel();
					token.ThrowIfCancellationRequested();
				};

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().AllowingExceptions().Nothing()
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates nothing after a warm-up allowing exceptions,
					             but it could not be verified, because the evaluation was already canceled
					             """);
				await That(calls).IsEqualTo(1)
					.Because("a cancellation aborts the expectation, even when exceptions are allowed");
			}

			[Test]
			public async Task WhenWarmUpOfDelegateWithValueThrows_ShouldFailWithoutMeasuring()
			{
				int calls = 0;
				Func<int> @delegate = () => calls++ == 0 ? throw new MyException() : calls;

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates nothing after a warm-up,
					              but it did throw a MyException in the warm-up:
					                {nameof(WhenWarmUpOfDelegateWithValueThrows_ShouldFailWithoutMeasuring)}
					              """);
				await That(calls).IsEqualTo(1);
			}

			[Test]
			public async Task WhenWarmUpOfDelegateWithValueThrowsAndExceptionsAreAllowed_ShouldMeasureTheSecondInvocation()
			{
				int calls = 0;
				Func<int> @delegate = () => calls++ == 0 ? throw new MyException() : calls;

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().AllowingExceptions().Nothing();

				await That(Act).DoesNotThrow();
				await That(calls).IsEqualTo(2);
			}

			[Test]
			public async Task WhenWarmUpThrows_ShouldFailWithoutMeasuring()
			{
				int calls = 0;
				Action @delegate = () =>
				{
					if (calls++ == 0)
					{
						throw new MyException();
					}
				};

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates nothing after a warm-up,
					              but it did throw a MyException in the warm-up:
					                {nameof(WhenWarmUpThrows_ShouldFailWithoutMeasuring)}
					              """);
				await That(calls).IsEqualTo(1)
					.Because("an exception in the warm-up fails the expectation like one in the measured invocation");
			}

			[Test]
			public async Task WhenWarmUpThrowsAndExceptionsAreAllowed_ShouldMeasureTheSecondInvocation()
			{
				int calls = 0;
				Action @delegate = () =>
				{
					if (calls++ == 0)
					{
						throw new MyException();
					}
				};

				async Task Act()
					=> await That(@delegate).Allocates().AfterWarmUp().AllowingExceptions().Nothing();

				await That(Act).DoesNotThrow()
					.Because("an allowed exception in the warm-up does not prevent the measured invocation");
				await That(calls).IsEqualTo(2);
			}

			[Test]
			public async Task WithoutWarmUp_ShouldInvokeTheDelegateOnce()
			{
				int calls = 0;
				Func<int> @delegate = () => ++calls;

				await That(@delegate).Allocates().Nothing();

				await That(calls).IsEqualTo(1)
					.Because("the measured value is exactly one invocation without a warm-up");
			}

			[Test]
			public async Task WithoutWarmUp_WhenDelegateAllocatesOnlyOnTheFirstCall_ShouldFail()
			{
				byte[]? cache = null;
				Action @delegate = () => cache ??= new byte[1000];

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates nothing,
					             but it allocated * bytes
					             """).AsWildcard();
			}
		}

		public sealed class AllowingExceptionsTests
		{
			[Test]
			public async Task WhenDelegateThrowsAndAllocatesTooMuch_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates nothing allowing exceptions,
					              but it allocated * bytes and did throw a MyException:
					                {nameof(WhenDelegateThrowsAndAllocatesTooMuch_ShouldFail)}
					              """).AsWildcard()
					.Because("the thrown exception is allocated by the delegate and therefore measured");
			}

			[Test]
			public async Task WhenDelegateThrowsWithinTheLimit_ShouldSucceed()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions().AtMost(1_000_000);

				await That(Act).DoesNotThrow()
					.Because("the allocations alone decide the outcome once exceptions are allowed");
			}

			[Test]
			public async Task WhenDelegateWithValueThrowsWithinTheLimit_ShouldSucceed()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions().AtMost(1_000_000);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldFailForAThrownException()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions(false).AtMost(1_000_000);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates at most 1000000 bytes,
					              but it did throw a MyException:
					                {nameof(WhenFalseIsSpecified_ShouldFailForAThrownException)}
					              """)
					.Because("an exception fails the expectation, as without the option");
			}

			[Test]
			public async Task WhenSpecifiedTwice_ShouldUseTheLastValue()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions().AllowingExceptions(false).AtMost(1_000_000);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates at most 1000000 bytes,
					              but it did throw a MyException:
					                {nameof(WhenSpecifiedTwice_ShouldUseTheLastValue)}
					              """);
			}
		}
	}
}
#endif
