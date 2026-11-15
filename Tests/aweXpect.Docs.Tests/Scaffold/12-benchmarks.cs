global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static string _upperCaseTitle = "HELP!";
	public static string _title = "Help!";
	public static int _tempo = 128;
	public static int _minimumTempo = 100;
	public static int[] _commonBeatsPerBar = [2, 3, 4, 6,];
	public static int _beatsPerBar = 6;
	public static BenchmarkAlbum _albumSubject = new();
	public static BenchmarkAlbum _albumExpectation = new();
	public static int[] _trackIds = Enumerable.Range(1, 1000).ToArray();
	public static int _minimumTrackCount = 1000;
	public static List<string> _trackTitles = ["Come Together", "Here Comes the Sun",];
	public static string _favoriteTitle = "Here Comes the Sun";
	public static string[] _bandMembers = ["Sting", "Andy", "Stewart",];
	public static string[] _expectedBandMembers = ["Sting", "Andy", "Stewart",];
	public static string[] _lineUp = ["Sting", "Andy", "Stewart",];
	public static string[] _shuffledLineUp = ["Sting", "Stewart", "Andy",];
	public static int _playCount = 42;
	public static string _alreadyPlayingMessage = "The track is already playing.";
	public static Action _playTwice = () => throw new InvalidOperationException(_alreadyPlayingMessage);
	public static Func<Task> _playTwiceAsync = () => Task.FromException(new InvalidOperationException(_alreadyPlayingMessage));
	public static BenchmarkAlbum _album = new();

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
