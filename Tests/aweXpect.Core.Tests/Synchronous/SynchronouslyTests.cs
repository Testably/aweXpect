using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Core.Tests.TestHelpers;
using static aweXpect.Synchronous.Synchronously;

// ReSharper disable InvokeAsExtensionMethod

namespace aweXpect.Core.Tests.Synchronous;

public class SynchronouslyTests
{
	[Test]
	public void ThatAll_WhenAllExpectationsAreMet_ShouldSucceed()
	{
		int value = 3;

		Verify(ThatAll(That(value).IsEqualTo(3), That(value).IsGreaterThan(2)));
	}

	[Test]
	public void ThatAll_WhenEvaluationYieldsOnABlockedSynchronizationContext_ShouldNotDeadlock()
	{
		bool completed = BlockedSynchronizationContext.Run(()
			=> Verify(ThatAll(That(async () => await Task.Yield()).DoesNotThrow())));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification"));
	}

	[Test]
	public void ThatAll_WhenOneExpectationFails_ShouldFail()
	{
		int value = 3;
		void Act() => Verify(ThatAll(That(value).IsEqualTo(3), That(value).IsEqualTo(4)));

		Verify(That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that value is equal to 3
			              [02] Expected that value is equal to 4
			             but
			              [02] it was 3, which differs by -1
			             """));
	}

	[Test]
	public async Task ThatAll_WhenOneExpectationFails_ShouldThrowTheSameExceptionTypeAsWhenAwaited()
	{
		int value = 3;
		void Act() => Verify(ThatAll(That(value).IsEqualTo(2)));
		async Task ActAsync() => await ThatAll(That(value).IsEqualTo(2));
		string expectedMessage = """
		                         Expected all of the following to succeed:
		                          [01] Expected that value is equal to 2
		                         but
		                          [01] it was 3, which differs by 1
		                         """;

		Exception awaited = await That(ActAsync).Throws().WithMessage(expectedMessage);

		await That(Act).ThrowsExactly(awaited.GetType()).WithMessage(expectedMessage)
			.Because("the synchronous verification fails the test like the awaited one");
	}

	[Test]
	public void ThatAny_WhenNoExpectationIsMet_ShouldFail()
	{
		int value = 3;
		void Act() => Verify(ThatAny(That(value).IsEqualTo(2), That(value).IsEqualTo(4)));

		Verify(That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			              [01] Expected that value is equal to 2
			              [02] Expected that value is equal to 4
			             but
			              [01] it was 3, which differs by 1
			              [02] it was 3, which differs by -1
			             """));
	}

	[Test]
	public void ThatAny_WhenOneExpectationIsMet_ShouldSucceed()
	{
		int value = 3;

		Verify(ThatAny(That(value).IsEqualTo(2), That(value).IsEqualTo(3)));
	}

	[Test]
	public void WhenActionDoesNotThrow_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = subject.Bar;
		Verify(That(() => ThrowIf(value != 3)).DoesNotThrow());
	}

	[Test]
	public void WhenActionThrows_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => Verify(That(() => ThrowIf(value == 3)).DoesNotThrow());

		Verify(That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that () => ThrowIf(value == 3)
			             does not throw any exception,
			             but it did throw a MyException:
			               WhenActionThrows_ShouldFail
			             """));
	}

	[Test]
	public void WhenAndResultFails_ShouldFail()
	{
		void Act() => Verify(That(() => ThrowIf(true)).ExecutesIn().AtMost(TimeSpan.FromMilliseconds(500))
			.Because("it must be fast"));

		Verify(That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that () => ThrowIf(true)
			             executes in at most 0:00.500, because it must be fast,
			             but it did throw a MyException:
			               WhenAndResultFails_ShouldFail
			             """));
	}

	[Test]
	public void WhenEvaluationIsMetRightAwayOnABlockedSynchronizationContext_WithValue_ShouldReturnTheValue()
	{
		int value = 0;

		bool completed = BlockedSynchronizationContext.Run(() => value = Verify(That(42).IsEqualTo(42)));

		Verify(That(completed).IsTrue());
		Verify(That(value).IsEqualTo(42));
	}

	[Test]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_ShouldNotDeadlock()
	{
		bool completed = BlockedSynchronizationContext.Run(()
			=> Verify(That(async () => await Task.Yield()).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification"));
	}

	[Test]
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

	[Test]
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

	[Test]
	public void WhenEvaluationYieldsOnABlockedTaskScheduler_ShouldNotDeadlock()
	{
		bool completed = BlockedTaskScheduler.Run(()
			=> Verify(That(async () => await Task.Yield()).DoesNotThrow()));

		Verify(That(completed).IsTrue()
			.Because("the continuation must not wait for the scheduler that is blocked by the synchronous verification"));
	}

	[Test]
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

	[Test]
	public void WhenPropertyValuesMatch_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => Verify(That(value).IsEqualTo(2));

		Verify(That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that value
			             is equal to 2,
			             but it was 3, which differs by 1
			             """));
	}

	[Test]
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
