namespace aweXpect.Core.Tests.TestHelpers;

public sealed class ThrowingMessageException : Exception
{
	public override string Message => throw new InvalidOperationException("the Message getter failed");
}
