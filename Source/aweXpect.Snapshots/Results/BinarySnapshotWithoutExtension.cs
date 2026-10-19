using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     A snapshot expectation of binary data that still needs its file extension, e.g.
///     <c>.MatchesSnapshot().WithExtension("png")</c>.
/// </summary>
/// <remarks>
///     Binary data does not tell which kind of file it is, so it is no expectation until <see cref="WithExtension" />
///     sets the extension; awaiting it before does not compile.
/// </remarks>
public sealed class BinarySnapshotWithoutExtension<TType, TThat>
{
	private readonly Func<string, BinarySnapshotResult<TType, TThat>> _create;

	internal BinarySnapshotWithoutExtension(Func<string, BinarySnapshotResult<TType, TThat>> create)
	{
		_create = create;
	}

	/// <summary>
	///     Stores the snapshot with the file <paramref name="extension" />, e.g. <c>png</c> in
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.png</c>, so that it can be opened with the matching
	///     application.
	/// </summary>
	/// <param name="extension">The file extension without a leading dot, e.g. <c>png</c>, <c>pdf</c> or <c>bin</c>.</param>
	/// <remarks>
	///     The pending file keeps the extension as well, e.g.
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.pending.png</c>.
	///     The extension is kept as given, and a store receives it as <see cref="SnapshotId.Extension" />.<br />
	///     It cannot be empty, contain a <c>.</c>, whitespace, control characters or any of
	///     <c>&lt; &gt; : " / \ | ? * % @</c>, or be <c>pending</c>.<br />
	///     A snapshot that was already used with another extension in the same test run fails.
	/// </remarks>
	public BinarySnapshotResult<TType, TThat> WithExtension(string extension)
	{
		SnapshotExtension.ThrowIfInvalid(extension);
		return _create(extension);
	}

	/// <summary>
	///     Makes awaiting the expectation without <see cref="WithExtension" /> a compile-time error.
	/// </summary>
	[Obsolete("A snapshot of binary data needs a file extension, e.g. .MatchesSnapshot().WithExtension(\"png\").",
		true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public TaskAwaiter GetAwaiter()
		=> throw new NotSupportedException(
			"A snapshot of binary data needs a file extension, e.g. .MatchesSnapshot().WithExtension(\"png\").");
}
