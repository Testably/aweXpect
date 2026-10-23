// The teaser of an extension leaves out what "Write your own extension" shows in full.
global using aweXpect.Core;
global using aweXpect.Core.Constraints;
global using aweXpect.Core.Extending;
global using aweXpect.Results;
global using static Snippets.Prelude;
using System.Text;

namespace Snippets;

internal static class Prelude
{
	public static CancellationToken cancellationToken;
	public static string message = "";
	public static int result;
	public static Guid id;
	public static Artists _artists = new();
	public static Radio _radio = new();
	public static Player _player = new();
	public static Track track = new();

	public class Artist
	{
		public bool IsActive { get; set; }
	}

	public class Artists
	{
		public Task<Artist> GetAsync(Guid id) => Task.FromResult(new Artist());
	}

	public class Song
	{
		public bool IsLive { get; set; }
	}

	public class Radio
	{
		public async IAsyncEnumerable<Song> StreamAsync()
		{
			await Task.Yield();
			yield return new Song();
		}
	}

	public class Player
	{
		public Task SetVolumeAsync(int volume) => Task.CompletedTask;
	}
}

public class Track;

public sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<Track>(it, grammars),
		IValueConstraint<Track?>
{
	public ConstraintResult IsMetBy(Track? actual) => this;

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
}
