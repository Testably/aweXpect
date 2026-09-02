namespace aweXpect.Core.Tests;

public sealed class FailTests
{
	[Theory]
	[AutoData]
	public async Task Test_ShouldThrowException(string reason)
	{
		void Act() => Fail.Test(reason);

		await That(Act).Throws<XunitException>()
			.WithMessage(reason);
	}

	[Theory]
	[AutoData]
	public async Task Test_WithInnerException_ShouldForwardInnerException(string reason)
	{
		Exception innerException = new InvalidOperationException("my inner exception");

		void Act() => Fail.Test(reason, innerException);

		await That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(innerException));
	}

	[Theory]
	[InlineAutoData(true)]
	[InlineAutoData(false)]
	public async Task Unless_ShouldThrowException(bool condition, string reason)
	{
		void Act() => Fail.Unless(condition, reason);

		await That(Act).Throws<XunitException>().OnlyIf(!condition)
			.WithMessage(reason);
	}

	[Theory]
	[AutoData]
	public async Task Unless_WithInnerException_WhenConditionIsFalse_ShouldForwardInnerException(string reason)
	{
		Exception innerException = new InvalidOperationException("my inner exception");

		void Act() => Fail.Unless(false, reason, innerException);

		await That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(innerException));
	}

	[Theory]
	[InlineAutoData(true)]
	[InlineAutoData(false)]
	public async Task When_ShouldThrowException(bool condition, string reason)
	{
		void Act() => Fail.When(condition, reason);

		await That(Act).Throws<XunitException>().OnlyIf(condition)
			.WithMessage(reason);
	}

	[Theory]
	[AutoData]
	public async Task When_WithInnerException_WhenConditionIsTrue_ShouldForwardInnerException(string reason)
	{
		Exception innerException = new InvalidOperationException("my inner exception");

		void Act() => Fail.When(true, reason, innerException);

		await That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(innerException));
	}
}
