namespace aweXpect.Snapshots;

/// <summary>
///     The text that replaces a masked value in an object snapshot.
/// </summary>
/// <remarks>
///     The formatter writes a replacement like any other value, so a <see cref="string" /> would be written in quotes.
///     An object without members is written by its <see cref="ToString" /> instead, which keeps the placeholder as it is,
///     e.g. <c>Id = {Id_1}</c>, and needs no reflection.
/// </remarks>
internal sealed class SnapshotPlaceholder(string text)
{
	public override string ToString() => text;
}
