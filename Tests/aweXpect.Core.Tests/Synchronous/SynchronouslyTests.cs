using System.Runtime.CompilerServices;
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
