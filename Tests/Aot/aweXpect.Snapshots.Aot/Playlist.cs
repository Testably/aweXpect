using System.Collections.Generic;

namespace aweXpect.Snapshots.Aot;

internal sealed class Playlist
{
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public Dictionary<string, int> Plays { get; set; } = [];
	public HashSet<int> Ratings { get; set; } = [];
	public List<Song> Songs { get; set; } = [];
}

internal sealed class Song
{
	public int Id { get; set; }
	public string Title { get; set; } = "";
}
