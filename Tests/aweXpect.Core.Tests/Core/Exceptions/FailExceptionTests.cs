namespace aweXpect.Core.Tests.Core.Exceptions;

public sealed class FailExceptionTests
{
	[Theory]
	[AutoData]
	public async Task InnerException_ShouldBeSet(string message)
	{
		Exception innerException = new InvalidOperationException("my inner exception");

		FailException subject = new(message, innerException);

		await That(subject.Message).IsEqualTo(message);
		await That(subject.InnerException).IsSameAs(innerException);
	}

	[Theory]
	[AutoData]
	public async Task Message_ShouldBeSet(string message)
	{
		FailException subject = new(message);

		await That(subject.Message).IsEqualTo(message);
	}
}
