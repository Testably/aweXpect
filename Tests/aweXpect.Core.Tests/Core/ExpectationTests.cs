namespace aweXpect.Core.Tests.Core;

public sealed class ExpectationTests
{
	[Test]
	[Arguments(false, true)]
	[Arguments(true, false)]
	[Arguments(false, false)]
	public async Task And_ShouldFailWhenAnyArgumentFails(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).And.IsEqualTo(b);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that true
			              is {a} and is {b},
			              but it was True
			              """);
	}

	[Test]
	[Arguments(true, true)]
	public async Task And_ShouldRequireBothArgumentsToSucceed(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).And.IsEqualTo(b);

		await That(Act).DoesNotThrow();
	}

	[Test]
	[Arguments(false, false)]
	public async Task Or_ShouldFailWhenBothArgumentsFail(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).Or.IsEqualTo(b);

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that true
			              is {a} or is {b},
			              but it was True
			              """);
	}

	[Test]
	[Arguments(false, true)]
	[Arguments(true, false)]
	[Arguments(true, true)]
	public async Task Or_ShouldRequireAnyArgumentToSucceed(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).Or.IsEqualTo(b);

		await That(Act).DoesNotThrow();
	}
}
