global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static Album album = new("Abbey Road");
	public static Album expected = new("Abbey Road");
	public static Album unexpected = new("Revolver");
}

public class Artist
{
	public DateTime BornOn { get; set; }
}

public class TrackId;
