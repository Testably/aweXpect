namespace aweXpect.Snapshots;

/// <summary>
///     The content of a snapshot, or of the value that was compared with it.
/// </summary>
public sealed class SnapshotContent
{
	internal SnapshotContent(byte[] bytes, string? text)
	{
		Bytes = bytes;
		Text = text;
	}

	/// <summary>
	///     Creates the content of a snapshot of binary data, e.g. to test a <see cref="SnapshotComparer" />.
	/// </summary>
	public static SnapshotContent FromBytes(byte[] bytes)
		=> new(RuleArguments.ThrowIfNull(bytes, nameof(bytes)), null);

	/// <summary>
	///     Creates the content of a snapshot of the <paramref name="text" />, with the <see cref="Bytes" /> as a
	///     <see cref="SnapshotStore" /> keeps them, e.g. to test a <see cref="SnapshotComparer" />.
	/// </summary>
	public static SnapshotContent FromText(string text)
		=> new(SnapshotTextEncoding.Encode(RuleArguments.ThrowIfNull(text, nameof(text))), text);

	/// <summary>
	///     The bytes as a <see cref="SnapshotStore" /> keeps them, so that writing them to a file gives the same file as
	///     the store.
	/// </summary>
	/// <remarks>
	///     Text is encoded as UTF-8 without a byte order mark and ends with an additional line break. The array is the one
	///     that the store received, so it must not be changed.
	/// </remarks>
	public byte[] Bytes { get; }

	/// <summary>
	///     Whether it is the text of a string or an object, and not binary data.
	/// </summary>
	public bool IsText => Text is not null;

	/// <summary>
	///     The text that was compared, without the additional line break of the <see cref="Bytes" />, or
	///     <see langword="null" /> for binary data.
	/// </summary>
	public string? Text { get; }
}
