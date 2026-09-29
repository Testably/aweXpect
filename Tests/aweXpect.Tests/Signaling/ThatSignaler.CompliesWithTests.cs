using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed class CompliesWithTests
	{
		[Fact]
		public async Task CompliesWith_DidNotSignal_WhenNotSignaled_ShouldReturnTheSignaler()
		{
			Signaler signaler = new();

			Signaler result = await That(signaler).CompliesWith(it => it.DidNotSignal().Within(50.Milliseconds()));

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task CompliesWith_Signaled_WhenSignaled_ShouldReturnTheSignaler()
		{
			Signaler signaler = new();
			signaler.Signal();

			Signaler result = await That(signaler).CompliesWith(it => it.Signaled());

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task CompliesWith_SignaledWithParameter_WhenSignaledWithMatchingParameter_ShouldReturnTheSignaler()
		{
			Signaler<int> signaler = new();
			signaler.Signal(42);

			Signaler<int> result = await That(signaler).CompliesWith(it => it.Signaled().With(p => p == 42));

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task DoesNotComplyWith_DidNotSignal_WhenSignaled_ShouldReturnTheSignaler()
		{
			Signaler signaler = new();
			signaler.Signal();

			Signaler result = await That(signaler).DoesNotComplyWith(it => it.DidNotSignal());

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task DoesNotComplyWith_DidNotSignalTimes_WhenSignaledOftenEnough_ShouldReturnTheSignaler()
		{
			Signaler signaler = new();
			signaler.Signal();
			signaler.Signal();

			Signaler result = await That(signaler).DoesNotComplyWith(it => it.DidNotSignal(2.Times()));

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task DoesNotComplyWith_DidNotSignalWithParameter_WhenSignaledWithMatchingParameter_ShouldReturnTheSignaler()
		{
			Signaler<int> signaler = new();
			signaler.Signal(42);

			Signaler<int> result =
				await That(signaler).DoesNotComplyWith(it => it.DidNotSignal().With(p => p == 42));

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task DoesNotComplyWith_Signaled_WhenNotSignaled_ShouldReturnTheSignaler()
		{
			Signaler signaler = new();

			Signaler result =
				await That(signaler).DoesNotComplyWith(it => it.Signaled().Within(50.Milliseconds()));

			await That(result).IsSameAs(signaler);
		}

		[Fact]
		public async Task DoesNotComplyWith_SignaledWithParameter_WhenNotSignaledWithMatchingParameter_ShouldReturnTheSignaler()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);

			Signaler<int> result = await That(signaler)
				.DoesNotComplyWith(it => it.Signaled().With(p => p == 42).Within(50.Milliseconds()));

			await That(result).IsSameAs(signaler);
		}
	}
}
