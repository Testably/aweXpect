namespace aweXpect.Core.Tests.Core;

public sealed class ThatBoolSubjectTests
{
	[Fact]
	public async Task WhenAwaitedWithoutExpectation_AndFalse_ShouldFail()
	{
		bool subject = false;

		async Task Act()
		{
#pragma warning disable aweXpect0001
			ThatBoolSubject sut = That(subject);
#pragma warning restore aweXpect0001
			await sut;
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is True,
			             but it was False
			             """);
	}

	[Fact]
	public async Task WhenAwaitedWithoutExpectation_AndTrue_ShouldSucceed()
	{
		bool subject = true;

		async Task Act()
		{
#pragma warning disable aweXpect0001
			ThatBoolSubject sut = That(subject);
#pragma warning restore aweXpect0001
			await sut;
		}

		await That(Act).DoesNotThrow();
	}
}
