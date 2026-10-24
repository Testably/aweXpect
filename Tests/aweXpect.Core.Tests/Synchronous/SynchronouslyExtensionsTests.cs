using System.Runtime.CompilerServices;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Synchronous;

// ReSharper disable InvokeAsExtensionMethod

namespace aweXpect.Core.Tests.Synchronous;

public class SynchronouslyExtensionsTests
{
	[Test]
	public void ThatAll_WhenAllExpectationsAreMet_ShouldSucceed()
	{
		int value = 3;

		ThatAll(That(value).IsEqualTo(3), That(value).IsGreaterThan(2)).VerifySynchronously();
	}

	[Test]
	public void ThatAll_WhenOneExpectationFails_ShouldFail()
	{
		int value = 3;
		void Act() => ThatAll(That(value).IsEqualTo(3), That(value).IsEqualTo(4)).VerifySynchronously();

		That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that value is equal to 3
			              [02] Expected that value is equal to 4
			             but
			              [02] it was 3, which differs by -1
			             """).VerifySynchronously();
	}

	[Test]
	public async Task ThatAll_WhenOneExpectationFails_ShouldThrowTheSameExceptionTypeAsWhenAwaited()
	{
		int value = 3;
		void Act() => ThatAll(That(value).IsEqualTo(2)).VerifySynchronously();
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
		void Act() => ThatAny(That(value).IsEqualTo(2), That(value).IsEqualTo(4)).VerifySynchronously();

		That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			              [01] Expected that value is equal to 2
			              [02] Expected that value is equal to 4
			             but
			              [01] it was 3, which differs by 1
			              [02] it was 3, which differs by -1
			             """).VerifySynchronously();
	}

	[Test]
	public void ThatAny_WhenOneExpectationIsMet_ShouldSucceed()
	{
		int value = 3;

		ThatAny(That(value).IsEqualTo(2), That(value).IsEqualTo(3)).VerifySynchronously();
	}

	[Test]
	public void WhenActionDoesNotThrow_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = subject.Bar;
		That(() => ThrowIf(value != 3)).DoesNotThrow().VerifySynchronously();
	}

	[Test]
	public void WhenActionThrows_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => That(() => ThrowIf(value == 3)).DoesNotThrow().VerifySynchronously();

		That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that () => ThrowIf(value == 3)
			             does not throw any exception,
			             but it did throw a MyException:
			               WhenActionThrows_ShouldFail
			             """).VerifySynchronously();
	}

	[Test]
	public void WhenAndResultFails_ShouldFail()
	{
		void Act() => That(() => ThrowIf(true)).ExecutesIn().AtMost(TimeSpan.FromMilliseconds(500))
			.Because("it must be fast").VerifySynchronously();

		That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that () => ThrowIf(true)
			             executes in at most 0:00.500, because it must be fast,
			             but it did throw a MyException:
			               WhenAndResultFails_ShouldFail
			             """).VerifySynchronously();
	}

	[Test]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_ShouldNotDeadlock()
	{
		bool completed = BlockedSynchronizationContext.Run(()
			=> That(async () => await Task.Yield()).DoesNotThrow().VerifySynchronously());

		That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification")
			.VerifySynchronously();
	}

	[Test]
	public void WhenEvaluationYieldsOnABlockedSynchronizationContext_WithValue_ShouldNotDeadlock()
	{
		int value = 0;

		bool completed = BlockedSynchronizationContext.Run(() => value = That(async () =>
		{
			await Task.Yield();
			return 42;
		}).DoesNotThrow().VerifySynchronously());

		That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous verification")
			.VerifySynchronously();
		That(value).IsEqualTo(42).VerifySynchronously();
	}

	[Test]
	public void WhenPropertyValuesDiffer_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => That(value).IsEqualTo(2).VerifySynchronously();

		That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that value
			             is equal to 2,
			             but it was 3, which differs by 1
			             """).VerifySynchronously();
	}

	[Test]
	public void WhenPropertyValuesMatch_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = That(subject.Bar).IsEqualTo(3).VerifySynchronously();

		That(value).IsEqualTo(3).VerifySynchronously();
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
