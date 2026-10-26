namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);

	// The exceptions of the test framework that the adapter on the page throws.
	internal sealed class MyTestFailedException(string message, Exception? innerException = null)
		: Exception(message, innerException);

	internal sealed class MyTestSkippedException(string message) : Exception(message);

	internal sealed class MyTestInconclusiveException(string message) : Exception(message);

	// The context of the running test that the adapter on the page reads.
	internal sealed class MyTestContext(string className, string methodName, object?[] arguments)
	{
		public static MyTestContext? Current => null;
		public string ClassName { get; } = className;
		public object?[] ClassArguments { get; } = [];
		public string MethodName { get; } = methodName;
		public object?[] Arguments { get; } = arguments;
		public string? Id => null;
		public string? DisplayName => null;
		public int RetryCount => 0;
		public System.Threading.CancellationToken CancellationToken => default;
	}
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

namespace System.Diagnostics.CodeAnalysis
{
	// The adapter on the page needs this attribute, which is missing in net48, and on the other targets it only causes a
	// warning.
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
	{
		public bool ReturnValue { get; } = returnValue;
	}
}
