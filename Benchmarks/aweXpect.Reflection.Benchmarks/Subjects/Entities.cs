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
