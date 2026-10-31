// The page marks its tests with the `[Test]` attribute of TUnit.
global using static Snippets.Prelude;
global using TUnit.Core;

namespace Snippets;

internal static class Prelude
{
	public static bool result;
}

public class MusicLibrary
{
	public bool Contains(string title) => false;
}
