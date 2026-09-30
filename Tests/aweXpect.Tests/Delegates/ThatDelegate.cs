using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public class CustomException(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: Exception(message, innerException)
	{
		public string? Value { get; set; }
	}

	public class SubCustomException(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: CustomException(message, innerException);

	public class OtherException(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: Exception(message, innerException);

	public class OuterException(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: Exception(message, innerException);

	public class GenericException<T>(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: Exception(message, innerException);

	public class SubGenericException(
		[CallerMemberName] string message = "",
		Exception? innerException = null)
		: GenericException<int>(message, innerException);
}
