using System.Collections.Generic;

namespace aweXpect.Web.Samples;

internal class TrackStore
{
	private readonly Dictionary<int, Track> _tracks = new();

	public TrackStore()
	{
		_tracks.Add(1, new Track(1, "Let It Be", "The Beatles"));
		_tracks.Add(2, new Track(2, "Jóga", "Björk"));
	}

	public IEnumerable<Track> GetTracks() => _tracks.Values;
	public Track GetTrack(int id) => _tracks[id];
}
