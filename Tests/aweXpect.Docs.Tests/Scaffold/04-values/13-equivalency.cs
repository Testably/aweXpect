global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static Album album = new("Abbey Road");
	public static Album expected = new("Abbey Road");
	public static Album unexpected = new("Revolver");
	public static Playlist playlist = new();
	public static Playlist expectedPlaylist = new();
	public static TrackDto[] dtos = [];
	public static TrackEntity[] entities = [];
}

public class TrackDto
{
	public int Id { get; set; }
}

public class TrackEntity
{
	public int Key { get; set; }
}

internal class Playlist : List<Track>
{
	public string Name { get; set; } = "";
}

public class Artist
{
	public DateTime BornOn { get; set; }
}

public class TrackId;
