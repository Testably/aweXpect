global using static Snippets.Prelude;
using aweXpect.Signaling;

namespace Snippets;

internal static class Prelude
{
	public static Signaler<string> signaler = new();
	public static MyClass sut = new();
}
