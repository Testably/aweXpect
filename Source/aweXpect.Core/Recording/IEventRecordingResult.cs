using System;

namespace aweXpect.Recording;

/// <summary>
///     The result of an <see cref="IEventRecording{TSubject}" />.
/// </summary>
public interface IEventRecordingResult
{
	/// <summary>
	///     Gets the number of recorded events for <paramref name="eventName" /> that match the optional
	///     <paramref name="filter" />.
	/// </summary>
	int GetEventCount(string eventName, Func<object?[], bool>? filter = null);

	/// <summary>
	///     Returns a formatted string for the recorded events for <paramref name="eventName" />.
	/// </summary>
	string ToString(string eventName);
}
