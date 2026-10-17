using System.IO;

namespace aweXpect.Snapshots;

/// <summary>
///     Identifies a snapshot independently of where a <see cref="SnapshotStore" /> keeps it.
/// </summary>
/// <param name="SourceFile">
///     The path of the test file relative to the source root of the project with <c>/</c> as separator, e.g.
///     <c>Tests/Orders/OrderTests.cs</c>, so that it is the same on every machine and in deterministic builds. It is the
///     full path when the build of the test project recorded no source root, e.g. without source control information.
/// </param>
/// <param name="MemberName">The name of the test method.</param>
/// <param name="Name">
///     The name from <c>.Named(…)</c>, which tells several snapshots of the same test method apart, or
///     <see langword="null" />.
/// </param>
public sealed record SnapshotId(string SourceFile, string MemberName, string? Name = null)
{
	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c>, or <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;</c> with a
	///     <see cref="Name" />, where <c>&lt;Class&gt;</c> is the name of the test file without its extension.
	/// </summary>
	public override string ToString()
		=> Name is null
			? $"{Path.GetFileNameWithoutExtension(SourceFile)}.{MemberName}"
			: $"{Path.GetFileNameWithoutExtension(SourceFile)}.{MemberName}.{Name}";
}
