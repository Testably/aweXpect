namespace aweXpect.Recording;

/// <summary>
///     A recording of events on a subject of type <typeparamref name="TSubject" />.
/// </summary>
/// <remarks>
///     Only a recording created by <c>.Record().Events()</c> is supported. An expectation on its events waits for them
///     with <see cref="Core.Extending.EventRecordings" />.
/// </remarks>
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IEventRecording<TSubject>;
#pragma warning restore S2326
