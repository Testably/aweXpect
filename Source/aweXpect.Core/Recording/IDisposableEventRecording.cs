using System;

namespace aweXpect.Recording;

/// <summary>
///     A recording of events on a subject of type <typeparamref name="TSubject" /> that keeps recording across
///     multiple expectations until it is disposed.
/// </summary>
/// <remarks>
///     Disposing detaches the handlers from the subject; an expectation on the disposed recording fails, so that it
///     cannot silently answer from the events that were recorded until then.
/// </remarks>
public interface IDisposableEventRecording<TSubject> : IEventRecording<TSubject>, IDisposable;
