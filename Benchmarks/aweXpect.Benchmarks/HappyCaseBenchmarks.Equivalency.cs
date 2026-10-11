using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that two objects are equivalent to one another.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private static readonly string[] AbbeyRoadTitles =
	[
		"Come Together", "Something", "Maxwell's Silver Hammer", "Oh! Darling", "Octopus's Garden",
		"I Want You (She's So Heavy)", "Here Comes the Sun", "Because", "You Never Give Me Your Money", "Sun King",
		"Mean Mr. Mustard", "Polythene Pam", "She Came In Through the Bathroom Window", "Golden Slumbers",
		"Carry That Weight", "The End", "Her Majesty",
	];

	private readonly Album _albumExpectation = Album.Create();
	private readonly Album _albumSubject = Album.Create();

	[Benchmark]
	public async Task<Album> Equivalency_aweXpect()
		=> await Expect.That(_albumSubject).IsEquivalentTo(_albumExpectation);

	[Benchmark]
	public AndConstraint<ObjectAssertions> Equivalency_FluentAssertions()
		=> _albumSubject.Should().BeEquivalentTo(_albumExpectation);

	[Benchmark]
	public async Task<Album?> Equivalency_TUnit()
		=> await Assert.That(_albumSubject).IsEquivalentTo(_albumExpectation);

	public enum Genre
	{
		Pop,
		Rock,
	}

	public sealed class Album
	{
		public string Title { get; set; } = "";
		public Artist Artist { get; set; } = new();
		public DateTime ReleaseDate { get; set; }
		public Genre Genre { get; set; }
		public List<Track> Tracks { get; set; } = [];

		public static Album Create()
			=> new()
			{
				Title = "Abbey Road",
				Artist = new Artist
				{
					Name = "The Beatles",
					Label = "Apple Records",
				},
				ReleaseDate = new DateTime(1969, 9, 26),
				Genre = Genre.Rock,
				Tracks = AbbeyRoadTitles.Select((title, index) => new Track
				{
					Number = index + 1,
					Title = title,
					Duration = TimeSpan.FromSeconds(150 + (7 * index)),
				}).ToList(),
			};
	}

	public sealed class Artist
	{
		public string Name { get; set; } = "";
		public string Label { get; set; } = "";
	}

	public sealed class Track
	{
		public int Number { get; set; }
		public string Title { get; set; } = "";
		public TimeSpan Duration { get; set; }
	}
}
