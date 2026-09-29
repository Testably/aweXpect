using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core.Tests.TestHelpers;
using static aweXpect.Synchronous.Synchronously;

// ReSharper disable InvokeAsExtensionMethod

namespace aweXpect.Core.Tests.Synchronous;

public class SynchronouslyTests
{
	[Fact]
	public void WhenActionDoesNotThrow_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = subject.Bar;
		Verify(That(() => ThrowIf(value != 3)).DoesNotThrow());
	}

	[Fact]
	public void WhenActionThrows_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => Verify(That(() => ThrowIf(value == 3)).DoesNotThrow());

		Verify(That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => ThrowIf(value == 3)
			             does not throw any exception,
			             but it did throw a MyException:
			               WhenActionThrows_ShouldFail
			             """));
	}

	[Fact]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_ShouldNotDeadlock()
	{
		bool completed = BlockedSynchronizationContext.Run(()
			=> Verify(That(async () => await Task.Yield()).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification"));
	}

	[Fact]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_ShouldRestoreIt()
	{
		SynchronizationContext? contextAfterwards = null;
		SynchronizationContext? contextBefore = null;

		BlockedSynchronizationContext.Run(() =>
		{
			contextBefore = SynchronizationContext.Current;
			Verify(That(async () => await Task.Yield()).DoesNotThrow());
			contextAfterwards = SynchronizationContext.Current;
		});

		Verify(That(contextAfterwards).IsSameAs(contextBefore)
			.Because("the caller keeps running on its synchronization context after the verification"));
	}

	[Fact]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_WithValue_ShouldNotDeadlock()
	{
		int value = 0;

		bool completed = BlockedSynchronizationContext.Run(() => value = Verify(That(async () =>
		{
			await Task.Yield();
			return 42;
		}).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification"));
		Verify(That(value).IsEqualTo(42));
	}

	[Fact]
	public void WhenEvaluationYieldsOnABlockedTaskScheduler_ShouldNotDeadlock()
	{
		bool completed = BlockedTaskScheduler.Run(()
			=> Verify(That(async () => await Task.Yield()).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the scheduler that is blocked by the synchronous verification"));
	}

	[Fact]
	public void WhenEvaluationYieldsOnABlockedTaskScheduler_WithValue_ShouldNotDeadlock()
	{
		int value = 0;

		bool completed = BlockedTaskScheduler.Run(() => value = Verify(That(async () =>
		{
			await Task.Yield();
			return 42;
		}).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the scheduler that is blocked by the synchronous verification"));
		Verify(That(value).IsEqualTo(42));
	}

	[Fact]
	public void WhenPropertyValuesMatch_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => Verify(That(value).IsEqualTo(2));

		Verify(That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that value
			             is equal to 2,
			             but it was 3, which differs by 1
			             """));
	}

	[Fact]
	public void WhenPropertyValuesMatch_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = Verify(That(subject.Bar).IsEqualTo(3));

		Verify(That(value).IsEqualTo(3));
	}

	private static void ThrowIf(bool condition, [CallerMemberName] string message = "")
	{
		if (condition)
		{
			throw new MyException(message);
		}
	}

	private ref struct Foo
	{
		public int Bar;
	}
}
