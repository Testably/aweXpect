namespace aweXpect.Recording;

/// <summary>
///     An event of a type that an <see cref="EventRecorder" /> can attach to.
/// </summary>
internal interface IRecordableEvent
{
	string Name { get; }
}
