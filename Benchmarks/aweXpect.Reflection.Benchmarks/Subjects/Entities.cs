namespace aweXpect.Reflection.Benchmarks.Subjects;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityAttribute : Attribute;

[Entity]
public sealed class Album
{
	public string Title { get; set; } = "";
}

[Entity]
public sealed class Artist
{
	public string Name { get; set; } = "";
}

[Entity]
public sealed class Track
{
	public string Title { get; set; } = "";
	public TimeSpan Duration { get; set; }
}

public sealed class AlbumRepository
{
	public static async Task SaveAsync(Album album) => await Task.Yield();

	public static async Task DeleteAsync(Album album) => await Task.Yield();

	public static async Task AddTrackAsync(Album album, Track track) => await Task.Yield();
}
