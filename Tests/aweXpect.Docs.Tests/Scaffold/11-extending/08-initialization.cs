namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);

	// The exceptions of the test framework that the adapter on the page throws.
	internal sealed class MyTestFailedException(string message, Exception? innerException = null)
		: Exception(message, innerException);

	internal sealed class MyTestSkippedException(string message) : Exception(message);

	internal sealed class MyTestInconclusiveException(string message) : Exception(message);
}

namespace System.Runtime.CompilerServices
{
	// The page tells to declare this attribute on targets that miss it, and on the others it only causes a warning.
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class ModuleInitializerAttribute : Attribute;

	// The record needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only causes
	// a warning.
	internal static class IsExternalInit;
}
