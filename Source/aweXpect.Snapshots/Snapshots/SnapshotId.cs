using System.IO;

namespace aweXpect.Snapshots;

/// <summary>
///     Identifies a snapshot independently of where a <see cref="SnapshotStore" /> keeps it.
/// </summary>
/// <param name="TestFilePath">The path of the test file, as the compiler sets it for the caller.</param>
/// <param name="MemberName">The name of the test method.</param>
public sealed record SnapshotId(string TestFilePath, string MemberName)
{
	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c>, where <c>&lt;Class&gt;</c> is the name of the test file without
	///     its extension.
	/// </summary>
	public override string ToString() => $"{Path.GetFileNameWithoutExtension(TestFilePath)}.{MemberName}";
}
