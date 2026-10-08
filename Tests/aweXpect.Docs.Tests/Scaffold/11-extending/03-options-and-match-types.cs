// The "Extending aweXpect" page shows this import.
global using aweXpect.Core.Extending;

namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);
}

// The record needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only causes a
// warning.
namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit;
}
