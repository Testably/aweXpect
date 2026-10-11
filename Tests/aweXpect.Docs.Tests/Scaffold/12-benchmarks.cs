global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static string _stringSubject = "foo";
	public static string _stringExpectation = "FOO";
	public static int _intSubject = 2;
	public static int _intMinimum = 1;
	public static BenchmarkAlbum _albumSubject = new();
	public static BenchmarkAlbum _albumExpectation = new();
	public static IEnumerable<int> _enumerableSubject = Enumerable.Range(1, 1000);
	public static int _enumerableCount = 1000;
	public static List<string> _containsSubject = ["Come Together", "Here Comes the Sun",];
	public static string _containsExpected = "Here Comes the Sun";
	public static string[] _stringArraySubject = ["foo", "bar", "baz",];
	public static string[] _stringArrayExpectation = ["foo", "bar", "baz",];
	public static string[] _stringArrayAnyOrderSubject = ["foo", "bar", "baz",];
	public static string[] _stringArrayAnyOrderExpectation = ["baz", "foo", "bar",];
	public static int _satisfiesSubject = 42;
	public static string _throwsMessage = "The track is already playing.";
	public static Action _throwsSubject = () => throw new InvalidOperationException(_throwsMessage);
	public static Func<Task> _throwsAsyncSubject = () => Task.FromException(new InvalidOperationException(_throwsMessage));
	public static BenchmarkAlbum _multipleSubject = new();

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
