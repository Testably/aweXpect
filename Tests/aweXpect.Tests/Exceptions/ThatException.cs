namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public class CustomException(string message, Exception? innerException = null)
		: Exception(message, innerException);

	public class GenericException<T>(string message, Exception? innerException = null)
		: Exception(message, innerException);
}
