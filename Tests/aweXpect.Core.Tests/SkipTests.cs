namespace aweXpect.Core.Tests;

public sealed class SkipTests
{
	[Test]
	[AutoArguments]
	public async Task Test_ShouldThrowException(string reason)
	{
		void Act() => Skip.Test(reason);

		await That(Act).Throws<SkipException>()
			.WithMessage($"*{reason}*").AsWildcard();
	}

	[Test]
	[AutoArguments(true)]
	[AutoArguments(false)]
	public async Task Unless_ShouldThrowException(bool condition, string reason)
	{
		void Act() => Skip.Unless(condition, reason);

		await That(Act).Throws<SkipException>().OnlyIf(!condition)
			.WithMessage($"*{reason}*").AsWildcard();
	}

	[Test]
	[AutoArguments(true)]
	[AutoArguments(false)]
	public async Task When_ShouldThrowException(bool condition, string reason)
	{
		void Act() => Skip.When(condition, reason);

		await That(Act).Throws<SkipException>().OnlyIf(condition)
			.WithMessage($"*{reason}*").AsWildcard();
	}
}
