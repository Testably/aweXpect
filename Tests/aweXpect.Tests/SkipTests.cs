namespace aweXpect.Tests;

public class SkipTests
{
	[Test]
	[AutoArguments]
	public async Task ThrownException_ShouldContainReason(string message)
	{
		void Act()
			=> Skip.Test(message);

		await That(Act).Throws()
			.WithMessage($"*{message}*").AsWildcard();
	}

	[Test]
	[AutoArguments(true)]
	[AutoArguments(false)]
	public async Task Unless_ShouldThrowExceptionWhenTrue(bool condition, string message)
	{
		void Act()
			=> Skip.Unless(condition, message);

		await That(Act).Throws().OnlyIf(!condition)
			.WithMessage($"*{message}*").AsWildcard();
	}

	[Test]
	[AutoArguments(true)]
	[AutoArguments(false)]
	public async Task When_ShouldThrowExceptionWhenTrue(bool condition, string message)
	{
		void Act()
			=> Skip.When(condition, message);

		await That(Act).Throws().OnlyIf(condition)
			.WithMessage($"*{message}*").AsWildcard();
	}
}
