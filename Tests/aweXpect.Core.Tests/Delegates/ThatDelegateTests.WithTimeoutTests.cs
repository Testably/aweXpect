using System.Threading;
using aweXpect.Chronology;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WithTimeoutTests
	{
		[Test]
		public async Task WhenAsyncDelegateReturnsAfterTheTimeout_ShouldFail()
		{
			Func<Task> @delegate = () => Task.Delay(100.Milliseconds());

			async Task Act()
				=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw any exception,
				             but it did not finish within 0:00.050
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("the task is abandoned once the timeout elapsed");
		}

		[Test]
		public async Task WhenSyncDelegateReturnsAfterTheTimeout_ShouldFail()
		{
			Action @delegate = () => Block(100.Milliseconds());

			async Task Act()
				=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw any exception,
				             but it did not finish within 0:00.050
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("a synchronous delegate that overran the timeout must fail like an asynchronous one");
		}

		[Test]
		public async Task WhenSyncDelegateWithValueReturnsAfterTheTimeout_ShouldFail()
		{
			Func<int> @delegate = () =>
			{
				Block(100.Milliseconds());
				return 1;
			};

			async Task Act()
				=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw any exception,
				             but it did not finish within 0:00.050
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("a synchronous delegate that overran the timeout must fail like an asynchronous one");
		}
	}

	/// <remarks>
	///     Blocks the calling thread like a synchronous delegate that cannot be interrupted. Its overrun is decided by
	///     its measured duration, which load can only lengthen.
	/// </remarks>
	private static void Block(TimeSpan duration)
	{
		using ManualResetEventSlim neverSet = new();
		_ = neverSet.Wait(duration);
	}
}
