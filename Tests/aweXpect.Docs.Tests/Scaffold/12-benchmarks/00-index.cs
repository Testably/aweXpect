global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static string _capsSongTitle = "RIO";
	public static string _expectedSongTitle = "Rio";
	public static int _currentBpm = 128;
	public static int _minimumBpm = 100;
	public static int[] _allowedBeatsPerBar = [2, 3, 4, 6,];
	public static int _songBeatsPerBar = 6;
	public static BenchmarkAlbum _albumSubject = new();
	public static BenchmarkAlbum _albumExpectation = new();
	public static int[] _trackIdsInLibrary = Enumerable.Range(1, 1000).ToArray();
	public static int _minTracksNeeded = 1000;
	public static List<string> _abbeyRoadTracks = ["Come Together", "Here Comes the Sun",];
	public static string _mostStreamedSong = "Here Comes the Sun";
	public static string[] _radioStationGenres = ["Pop", "Rap", "Ska",];
	public static string[] _scheduledStationGenres = ["Pop", "Rap", "Ska",];
	public static string[] _radioStationGenresAsListed = ["Pop", "Rap", "Ska",];
	public static string[] _radioStationGenresInOtherOrder = ["Ska", "Pop", "Rap",];
	public static int _playCountOfTrack = 42;
	public static string _playbackError = "The track is already playing.";
	public static Action _playSongTwice = () => throw new InvalidOperationException(_playbackError);
	public static Func<Task> _playSongTwiceAsync = () => Task.FromException(new InvalidOperationException(_playbackError));
	public static BenchmarkAlbum _abbeyRoadRecord = new();

	public enum Genre
	{
		Rock,
	}

	public class BenchmarkAlbum
	{
		public string Title { get; set; } = "Abbey Road";
		public BenchmarkArtist Artist { get; set; } = new();
		public Genre Genre { get; set; }
	}

	public class BenchmarkArtist
	{
		public string Name { get; set; } = "The Beatles";
	}

	public sealed class Player
	{
		public event EventHandler? Started;

		public void Play() => Started?.Invoke(this, EventArgs.Empty);
	}
}
