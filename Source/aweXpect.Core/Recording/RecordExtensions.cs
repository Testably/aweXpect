using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Metadata;

namespace aweXpect.Recording;

/// <summary>
///     Extension methods for creating <see cref="IEventRecording{TSubject}" />.
/// </summary>
public static class RecordExtensions
{
	/// <summary>
	///     Create a recording on the <paramref name="subject" />.
	/// </summary>
	/// <remarks>
	///     The events of the static type of the <paramref name="subject" /> are registered by the source generator, so
	///     that recording them does not depend on reflection when publishing with trimming or Native AOT enabled.
	/// </remarks>
	public static RecordingFactory<TSubject> Record<TSubject>([RequiresEventMetadata] this TSubject subject,
		[CallerArgumentExpression("subject")] string doNotPopulateThisValue = "")
		where TSubject : notnull
		=> new(subject, doNotPopulateThisValue);

	/// <summary>
	///     Keeps the <paramref name="recording" /> recording across multiple expectations until it is disposed.
	/// </summary>
	/// <remarks>
	///     Without it, the recording stops with the first expectation, and a further expectation on it fails instead of
	///     answering from the events that were recorded until then.
	/// </remarks>
	public static IDisposableEventRecording<TSubject> UntilDisposed<TSubject>(this IEventRecording<TSubject> recording)
		where TSubject : notnull
		=> recording is EventRecording<TSubject> eventRecording
			? eventRecording.UntilDisposed()
			: throw Tracing.WriteException(
				new NotSupportedException(
					$"Only a recording created by .Record().Events() supports .UntilDisposed(), but was {Formatter.Format(recording)}"));
}
