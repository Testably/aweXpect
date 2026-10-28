#if NET8_0_OR_GREATER
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Allocates
	{
		public sealed class ActionTests
		{
			[Test]
			public async Task WhenBecauseIsSpecifiedBeforeAnd_ShouldIncludeTheReason()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().Nothing().Because("it runs on the hot path")
						.And.DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates nothing and does not throw any exception, because it runs on the hot path,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocates_ShouldFail()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates nothing,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocatesNothing_ShouldSucceed()
			{
				int value = 0;
				Action @delegate = () => value++;

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).DoesNotThrow()
					.Because("incrementing a captured integer does not allocate, and aweXpect must not count its own allocations");
			}

			[Test]
			public async Task WhenDelegateExceedsTheTimeout_ShouldFail()
			{
				VirtualTimeProvider time = new();
				Action @delegate = () => time.Advance(200.Milliseconds());

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(1_000_000).WithTimeout(50.Milliseconds())
						.WithTimeProvider(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates at most 1000000 bytes,
					             but it did not finish within 0:00.050
					             """);
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(1_000_000);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates at most 1000000 bytes,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a delegate that crashed did not run to completion, so its allocations are no measurement");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).Allocates().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             allocates nothing,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithCancellationToken_WhenDelegateAllocatesNothing_ShouldSucceed()
			{
				int value = 0;
				Action<CancellationToken> @delegate = _ => value++;

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).DoesNotThrow()
					.Because("a delegate with a cancellation token is also synchronous");
			}
		}

		public sealed class AsyncTests
		{
			[Test]
			public async Task WhenDelegateReturnsTask_ShouldThrowNotSupportedException()
			{
				Func<Task> @delegate = () => Task.CompletedTask;

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"Allocates() is only supported for synchronous delegates, because an asynchronous delegate can continue on another thread, whose allocations are not measured.")
					.Because("the allocations after an await would be missed, so the expectation must not silently measure them wrong");
			}

			[Test]
			public async Task WhenDelegateReturnsTaskWithValue_ShouldThrowNotSupportedException()
			{
				Func<CancellationToken, Task<int>> @delegate = _ => Task.FromResult(1);

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"Allocates() is only supported for synchronous delegates, because an asynchronous delegate can continue on another thread, whose allocations are not measured.");
			}

			[Test]
			public async Task WhenDelegateReturnsValueTask_ShouldThrowNotSupportedException()
			{
				ValueTask Delegate() => new(Task.CompletedTask);

				async Task Act()
					=> await That(Delegate).Allocates().Nothing();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"Allocates() is only supported for synchronous delegates, because an asynchronous delegate can continue on another thread, whose allocations are not measured.");
			}

			[Test]
			public async Task WhenSubjectIsTask_ShouldThrowNotSupportedException()
			{
				Task subject = Task.CompletedTask;

				async Task Act()
					=> await That(subject).Allocates().Nothing();

				await That(Act).Throws<NotSupportedException>()
					.WithMessage(
						"Allocates() is only supported for synchronous delegates, because an asynchronous delegate can continue on another thread, whose allocations are not measured.");
			}
		}

		public sealed class CombinationTests
		{
			[Test]
			public async Task WhenCombinedWithDoesNotThrow_ShouldReportBoth()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().Nothing().And.DoesNotThrow();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates nothing and does not throw any exception,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenCombinedWithExecutesIn_ShouldSucceed()
			{
				int value = 0;
				Action @delegate = () => value++;

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtMost(5.Seconds()).And.Allocates().Nothing();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCombinedWithThrows_ShouldMeasureTheThrowingInvocation()
			{
				Action @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AllowingExceptions().AtLeast(1).And.Throws<MyException>();

				await That(Act).DoesNotThrow()
					.Because("the exception is allocated by the delegate, which both expectations see");
			}

			[Test]
			public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).Allocates().AtLeast(0).And.Allocates().AfterWarmUp().AtMost(100);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Allocates cannot be specified more than once.")
					.Because("the delegate is invoked once for all expectations, so their warm-up must not differ");
			}
		}

		public sealed class FuncValueTests
		{
			[Test]
			public async Task WhenDelegateAllocates_ShouldFail()
			{
				Func<byte[]> @delegate = () => new byte[1000];

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(100);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates at most 100 bytes,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocatesNothing_ShouldSucceed()
			{
				int value = 0;
				Func<int> @delegate = () => ++value;

				async Task Act()
					=> await That(@delegate).Allocates().Nothing();

				await That(Act).DoesNotThrow()
					.Because("returning a value type must not box it");
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				Func<int> @delegate = () => throw new MyException();

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(1_000_000);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              allocates at most 1000000 bytes,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Func<int>? subject = null;

				async Task Act()
					=> await That(subject!).Allocates().Nothing();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             allocates nothing,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithCancellationToken_WhenDelegateAllocates_ShouldSucceed()
			{
				Func<CancellationToken, byte[]> @delegate = _ => new byte[1000];

				async Task Act()
					=> await That(@delegate).Allocates().AtLeast(1000);

				await That(Act).DoesNotThrow()
					.Because("a delegate with a cancellation token is also synchronous");
			}
		}
	}
}
#endif
