using System.IO;

namespace aweXpect.Snapshots;

/// <summary>
///     Identifies a snapshot independently of where a <see cref="SnapshotStore" /> keeps it.
/// </summary>
/// <param name="TestFilePath">The path of the test file, as the compiler sets it for the caller.</param>
/// <param name="MemberName">The name of the test method.</param>
/// <param name="Name">
///     The name from <c>.Named(…)</c>, which tells several snapshots of the same test method apart, or
///     <see langword="null" />.
/// </param>
public sealed record SnapshotId(string TestFilePath, string MemberName, string? Name = null)
{
	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c>, or <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;</c> with a
	///     <see cref="Name" />, where <c>&lt;Class&gt;</c> is the name of the test file without its extension.
	/// </summary>
	public override string ToString()
		=> Name is null
			? $"{Path.GetFileNameWithoutExtension(TestFilePath)}.{MemberName}"
			: $"{Path.GetFileNameWithoutExtension(TestFilePath)}.{MemberName}.{Name}";
}
