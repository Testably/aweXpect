namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);
}

namespace System.Runtime.CompilerServices
{
	// The "Initialization" page tells to declare this attribute on targets that miss it, and on the others it only
	// causes a warning.
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class ModuleInitializerAttribute : Attribute;

	// The record needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only causes
	// a warning.
	internal static class IsExternalInit;
}
