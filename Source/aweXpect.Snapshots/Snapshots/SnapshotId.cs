using System.IO;

namespace aweXpect.Snapshots;

/// <summary>
///     Identifies a snapshot independently of where a <see cref="SnapshotStore" /> keeps it.
/// </summary>
/// <param name="SourceFile">
///     The path of the test file relative to the source root of the project with <c>/</c> as separator, e.g.
///     <c>Tests/Albums/AlbumTests.cs</c>, so that it is the same on every machine and in deterministic builds. It is the
///     full path when the build of the test project recorded no source root, e.g. without source control information.
/// </param>
/// <param name="MemberName">The name of the test method.</param>
/// <param name="Name">
///     The name from <c>.Named(…)</c> or <c>.NamedAfter(…)</c>, which tells several snapshots of the same test method apart, or
///     <see langword="null" />.
/// </param>
public sealed record SnapshotId(string SourceFile, string MemberName, string? Name = null)
{
	/// <summary>
	///     The environment that the snapshot is unique for, e.g. <c>net8.0@windows@x64</c>, or <see langword="null" />
	///     when every environment shares the snapshot.
	/// </summary>
	/// <remarks>
	///     It is set by <c>.UniqueFor</c>, e.g. <c>.UniqueFor.TargetFramework()</c>, and joins the target framework, the
	///     operating system and the architecture, in this order, with <c>@</c>. A store keeps it as one opaque value.
	/// </remarks>
	public string? Variant { get; init; }

	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c>, or <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;</c> with a
	///     <see cref="Name" />, where <c>&lt;Class&gt;</c> is the name of the test file without its extension, followed
	///     by <c>@&lt;Variant&gt;</c> with a <see cref="Variant" />.
	/// </summary>
	public override string ToString()
	{
		string id = Name is null
			? $"{Path.GetFileNameWithoutExtension(SourceFile)}.{MemberName}"
			: $"{Path.GetFileNameWithoutExtension(SourceFile)}.{MemberName}.{Name}";
		return Variant is null ? id : $"{id}@{Variant}";
	}
}
