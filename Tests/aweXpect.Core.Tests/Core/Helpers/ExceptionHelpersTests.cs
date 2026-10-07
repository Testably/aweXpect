namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class ExceptionHelpersTests
{
	[Test]
	public async Task Throws_WithGenericTypeDefinition_WhenADerivedExceptionIsThrown_ShouldSucceed()
	{
		void Delegate() => throw new DerivedGenericException();

		async Task Act()
			=> await That(Delegate).Throws(typeof(GenericException<>));

		await That(Act).DoesNotThrow()
			.Because("an open generic type matches every exception derived from one constructed from it");
	}

	[Test]
	public async Task Throws_WithGenericTypeDefinition_WhenAnotherExceptionIsThrown_ShouldFail()
	{
		void Delegate() => throw new OtherGenericException<int>();

		async Task Act()
			=> await That(Delegate).Throws(typeof(GenericException<>));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that Delegate
			             throws an ExceptionHelpersTests.GenericException<>,
			             but it did throw an ExceptionHelpersTests.OtherGenericException<int>:
			               other
			             """);
	}

	[Test]
	public async Task Throws_WithTypeThatIsNoException_ShouldThrowArgumentException()
	{
		void Delegate() { }

		async Task Act()
			=> await That(Delegate).Throws(typeof(string));

		await That(Act).Throws<ArgumentException>()
			.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix().And
			.WithParamName("type");
	}

	[Test]
	public async Task ThrowsExactly_WithGenericTypeDefinition_WhenAConstructedExceptionIsThrown_ShouldSucceed()
	{
		void Delegate() => throw new GenericException<int>();

		async Task Act()
			=> await That(Delegate).ThrowsExactly(typeof(GenericException<>));

		await That(Act).DoesNotThrow()
			.Because("an open generic type matches every exception constructed directly from it");
	}

	[Test]
	public async Task ThrowsExactly_WithGenericTypeDefinition_WhenADerivedExceptionIsThrown_ShouldFail()
	{
		void Delegate() => throw new DerivedGenericException();

		async Task Act()
			=> await That(Delegate).ThrowsExactly(typeof(GenericException<>));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that Delegate
			             throws exactly an ExceptionHelpersTests.GenericException<>,
			             but it did throw an ExceptionHelpersTests.DerivedGenericException:
			               generic
			             """);
	}

	[Test]
	public async Task ThrowsExactly_WithGenericTypeDefinition_WhenAnotherGenericExceptionIsThrown_ShouldFail()
	{
		void Delegate() => throw new OtherGenericException<int>();

		async Task Act()
			=> await That(Delegate).ThrowsExactly(typeof(GenericException<>));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that Delegate
			             throws exactly an ExceptionHelpersTests.GenericException<>,
			             but it did throw an ExceptionHelpersTests.OtherGenericException<int>:
			               other
			             """);
	}

	[Test]
	public async Task WithInner_WithGenericTypeDefinition_WhenThereIsNoInnerException_ShouldFail()
	{
		void Delegate() => throw new OtherGenericException<int>();

		async Task Act()
			=> await That(Delegate).Throws<OtherGenericException<int>>().WithInner(typeof(GenericException<>));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that Delegate
			             throws an ExceptionHelpersTests.OtherGenericException<int> with an inner ExceptionHelpersTests.GenericException<>,
			             but it had no inner exception
			             """);
	}

	private class GenericException<T>() : Exception("generic");

	private sealed class DerivedGenericException : GenericException<int>;

	private sealed class OtherGenericException<T>() : Exception("other");
}
