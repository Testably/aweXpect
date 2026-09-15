namespace aweXpect.Core.Tests.Core;

public sealed class ExpectationTests
{
	[Theory]
	[InlineData(false, true)]
	[InlineData(true, false)]
	[InlineData(false, false)]
	public async Task And_ShouldFailWhenAnyArgumentFails(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).And.IsEqualTo(b);

		await That(Act).Throws<XunitException>();
	}

	[Theory]
	[InlineData(true, true)]
	public async Task And_ShouldRequireBothArgumentsToSucceed(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).And.IsEqualTo(b);

		await That(Act).DoesNotThrow();
	}

	[Theory]
	[InlineData(false, false)]
	public async Task Or_ShouldFailWhenBothArgumentsFail(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).Or.IsEqualTo(b);

		await That(Act).Throws();
	}

	[Theory]
	[InlineData(false, true)]
	[InlineData(true, false)]
	[InlineData(true, true)]
	public async Task Or_ShouldRequireAnyArgumentToSucceed(bool a, bool b)
	{
		async Task Act()
			=> await That(true).IsEqualTo(a).Or.IsEqualTo(b);

		await That(Act).DoesNotThrow();
	}
}
