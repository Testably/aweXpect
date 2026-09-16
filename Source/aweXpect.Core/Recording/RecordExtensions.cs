using System.Runtime.CompilerServices;
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
}
