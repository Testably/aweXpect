// The page adds the static using, and writes its tests with xUnit.
global using static aweXpect.Expect;
global using static Snippets.Prelude;
global using Xunit;

namespace Snippets;

internal static class Prelude
{
	public static bool subject;

	public static bool IsInLibrary(string title) => false;
}
