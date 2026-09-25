using System.Runtime.CompilerServices;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Synchronous;

// ReSharper disable InvokeAsExtensionMethod

namespace aweXpect.Core.Tests.Synchronous;

public class SynchronouslyExtensionsTests
{
	[Fact]
	public void WhenActionDoesNotThrow_ShouldSucceed()
	{
		Foo subject = new()
		{
			Bar = 3,
		};

		int value = subject.Bar;
		That(() => ThrowIf(value != 3)).DoesNotThrow().VerifySynchronously();
	}

	[Fact]
	public void WhenActionThrows_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => That(() => ThrowIf(value == 3)).DoesNotThrow().VerifySynchronously();

		That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that () => ThrowIf(value == 3)
			             does not throw any exception,
			             but it did throw a MyException:
			               WhenActionThrows_ShouldFail
			             """).VerifySynchronously();
	}

	[Fact]
	public void WhenPropertyValuesMatch_ShouldFail()
	{
		Foo subject = new()
		{
			Bar = 3,
		};
		int value = subject.Bar;
		void Act() => That(value).IsEqualTo(2).VerifySynchronously();

		That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that value
			             is equal to 2,
			             but it was 3, which differs by 1
			             """).VerifySynchronously();
	}

	[Fact]
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
