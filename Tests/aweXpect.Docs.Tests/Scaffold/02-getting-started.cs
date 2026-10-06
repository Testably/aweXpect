// The page adds the static using, and marks its tests with the `[Test]` attribute of TUnit.
global using static aweXpect.Expect;
global using static Snippets.Prelude;
global using TUnit.Core;

namespace Snippets;

internal static class Prelude
{
	public static bool subject;

	public static bool IsInLibrary(string title) => false;
}
