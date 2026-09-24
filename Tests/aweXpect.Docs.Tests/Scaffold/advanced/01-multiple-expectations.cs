global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static AlbumWithLoader subject = new();

	public class AlbumWithLoader
	{
		public Task<string> LoadTitleAsync() => Task.FromResult("Dark Side of the Moon");
	}
}
