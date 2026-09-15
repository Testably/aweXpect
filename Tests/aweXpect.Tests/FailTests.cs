namespace aweXpect.Tests;

public class FailTests
{
	[Theory]
	[AutoData]
	public async Task ThrownException_ShouldContainReason(string message)
	{
		void Act()
			=> Fail.Test(message);

		await That(Act).Throws()
			.WithMessage($"*{message}*").AsWildcard();
	}

	[Theory]
	[InlineAutoData(true)]
	[InlineAutoData(false)]
	public async Task Unless_ShouldThrowExceptionWhenTrue(bool condition, string message)
	{
		void Act()
			=> Fail.Unless(condition, message);

		await That(Act).Throws().OnlyIf(!condition)
			.WithMessage($"*{message}*").AsWildcard();
	}

	[Theory]
	[InlineAutoData(true)]
	[InlineAutoData(false)]
	public async Task When_ShouldThrowExceptionWhenTrue(bool condition, string message)
	{
		void Act()
			=> Fail.When(condition, message);

		await That(Act).Throws().OnlyIf(condition)
			.WithMessage($"*{message}*").AsWildcard();
	}
}
