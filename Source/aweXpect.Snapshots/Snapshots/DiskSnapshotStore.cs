using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Keeps the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.txt</c> next to the test file and records a mismatch
///     in <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending.txt</c>, which can be renamed to accept it.
/// </summary>
/// <remarks>
///     The files are encoded as UTF-8 and the pending file ends with an additional line break, which is removed again
///     when the snapshot is read, so that an editor which adds a final line break does not change it.<br />
///     The files are accessed synchronously, as netstandard2.0 lacks the asynchronous methods of <see cref="File" />.<br />
///     A relative <see cref="SnapshotId.SourceFile" /> is resolved against the recorded source root, see
///     <see cref="SourceRoots" />.<br />
///     The method name and the name of the snapshot are escaped in the file name, see
///     <see cref="EscapeFileNamePart(string)" />, and a <see cref="SnapshotId.Variant" /> is appended after an <c>@</c>,
///     e.g. <c>&lt;Class&gt;.&lt;Method&gt;@net8.0@windows.snapshot.txt</c>. The <see cref="SnapshotId.Extension" />
///     replaces <c>txt</c>, e.g. <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending.json</c>.
/// </remarks>
internal sealed class DiskSnapshotStore : SnapshotStore
{
	public static readonly DiskSnapshotStore Instance = new();
	private const string PendingSuffix = ".snapshot.pending";
	private const string SnapshotSuffix = ".snapshot";
	private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

	private static readonly StringComparison PathComparison = Path.DirectorySeparatorChar == '\\'
		? StringComparison.OrdinalIgnoreCase
		: StringComparison.Ordinal;

	private ConcurrentDictionary<SnapshotId, (string BasePath, string Extension)> _listed =
		new(ClaimedSnapshots.Comparer);

	private DiskSnapshotStore()
	{
	}

	/// <remarks>
	///     The test file is resolved before the snapshot is read, so that an ambiguous source root is reported instead of
	///     reading the snapshot next to another test file with the same relative path.
	/// </remarks>
	public override ValueTask<string?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		SourceRoots.ThrowIfUnresolvable(id.SourceFile);
		string snapshotPath = GetSnapshotPath(id);
		if (!File.Exists(snapshotPath))
		{
			return new ValueTask<string?>((string?)null);
		}

		return new ValueTask<string?>(RemoveFinalLineBreak(File.ReadAllText(snapshotPath)));
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		File.WriteAllText(GetPendingPath(id), actual + Environment.NewLine, Utf8WithoutBom);
		return default;
	}

	/// <remarks>
	///     The snapshot is written like a pending file, so that the next run reads the <paramref name="actual" /> value
	///     unchanged. The accepted snapshot makes a leftover pending file obsolete, so one that cannot be deleted does not
	///     fail it; the next matching run deletes it.
	/// </remarks>
	public override ValueTask AcceptAsync(SnapshotId id, string actual, CancellationToken cancellationToken)
	{
		File.WriteAllText(GetSnapshotPath(id), actual + Environment.NewLine, Utf8WithoutBom);
		try
		{
			DeletePending(id);
		}
		catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
		{
			// The pending file is only a leftover, see the remarks.
		}

		return default;
	}

	public override ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		DeletePending(id);
		return default;
	}

	private static void DeletePending(SnapshotId id)
	{
		string pendingPath = GetPendingPath(id);
		if (File.Exists(pendingPath))
		{
			File.Delete(pendingPath);
		}
	}

	/// <remarks>
	///     A snapshot that <see cref="ListAsync" /> listed is described by the file it was found in, which is the pending
	///     file when only a mismatch was recorded.
	/// </remarks>
	public override string Describe(SnapshotId id)
	{
		if (!_listed.TryGetValue(id, out (string BasePath, string Extension) listed))
		{
			return GetSnapshotPath(id);
		}

		string snapshotPath = GetSnapshotPath(listed.BasePath, listed.Extension);
		string pendingPath = GetPendingPath(listed.BasePath, listed.Extension);
		return !File.Exists(snapshotPath) && File.Exists(pendingPath) ? pendingPath : snapshotPath;
	}

	public override string DescribeMismatch(SnapshotId id) => GetPendingPath(id);

	/// <remarks>
	///     Lists the snapshot and pending files below the directories of the test projects that the build targets
	///     recorded, but not in output directories (<c>bin</c>, <c>obj</c>), hidden directories or the directories of
	///     another project, so that the snapshots of another test project are never listed.<br />
	///     A file is only listed when it can be read back into the id that it was written for, see
	///     <see cref="TryGetId" />. The file of each listed id is remembered, so that <see cref="DeleteAsync" /> deletes
	///     exactly that file, also when its test file was deleted and several source roots are recorded.
	/// </remarks>
	public override ValueTask<IReadOnlyList<SnapshotId>?> ListAsync(CancellationToken cancellationToken)
	{
		ConcurrentDictionary<SnapshotId, (string BasePath, string Extension)> listed = new(ClaimedSnapshots.Comparer);
		foreach (string projectDirectory in SourceRoots.GetProjectDirectories().Where(Directory.Exists))
		{
			CollectSnapshots(projectDirectory, listed, cancellationToken);
		}

		_listed = listed;
		return new ValueTask<IReadOnlyList<SnapshotId>?>(listed.Keys.ToArray());
	}

	/// <remarks>
	///     A snapshot that <see cref="ListAsync" /> listed is deleted in the file it was found in.
	/// </remarks>
	public override ValueTask DeleteAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		(string basePath, string extension) = _listed.TryGetValue(id, out (string, string) listed)
			? listed
			: (GetBasePath(id), id.Extension);
		string[] paths = [GetSnapshotPath(basePath, extension), GetPendingPath(basePath, extension),];
		foreach (string path in paths.Where(File.Exists))
		{
			File.Delete(path);
		}

		return default;
	}

	private static void CollectSnapshots(string directory,
		ConcurrentDictionary<SnapshotId, (string BasePath, string Extension)> listed,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		string[] files = Directory.GetFiles(directory);
		foreach (string file in files)
		{
			(string BaseName, string Extension)? snapshotFile = ParseFileName(Path.GetFileName(file));
			if (snapshotFile is { } parsed &&
			    TryGetId(directory, parsed.BaseName, parsed.Extension, files, out SnapshotId? id))
			{
				listed.TryAdd(id, (Path.Combine(directory, parsed.BaseName), parsed.Extension));
			}
		}

		foreach (string subdirectory in Directory.GetDirectories(directory))
		{
			string name = Path.GetFileName(subdirectory);
			if ((name.Length > 0 && name[0] == '.') ||
			    name.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
			    name.Equals("obj", StringComparison.OrdinalIgnoreCase) ||
			    Directory.EnumerateFiles(subdirectory, "*.*proj").Any())
			{
				continue;
			}

			CollectSnapshots(subdirectory, listed, cancellationToken);
		}
	}

	/// <summary>
	///     Splits the name of a snapshot or pending file into the base name and the extension, or returns
	///     <see langword="null" /> for any other file.
	/// </summary>
	/// <remarks>
	///     The extension is the part after the last <c>.</c>, as it cannot contain one, so
	///     <c>&lt;Base&gt;.snapshot.pending.json</c> is always the pending file of <c>&lt;Base&gt;.snapshot.json</c>, never a
	///     snapshot with the extension <c>pending.json</c>. An extension that <c>.WithExtension(…)</c> does not allow,
	///     e.g. <c>pending</c>, is no snapshot file.
	/// </remarks>
	private static (string BaseName, string Extension)? ParseFileName(string fileName)
	{
		int extensionStart = fileName.LastIndexOf('.') + 1;
		string extension = fileName.Substring(extensionStart);
		if (extensionStart == 0 || !SnapshotExtension.IsValid(extension))
		{
			return null;
		}

		string withoutExtension = fileName.Substring(0, extensionStart - 1);
		string? baseName = RemoveSuffix(withoutExtension, PendingSuffix) ?? RemoveSuffix(withoutExtension, SnapshotSuffix);
		return baseName is null ? null : (baseName, extension);
	}

	private static string? RemoveSuffix(string fileName, string suffix)
		=> fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
			? fileName.Substring(0, fileName.Length - suffix.Length)
			: null;

	/// <summary>
	///     Reads the <paramref name="baseName" /> of a snapshot file back into the id that it was written for.
	/// </summary>
	/// <remarks>
	///     The class part is the name of a test file in the same directory, which may contain dots. The method and the
	///     name follow, with their dots escaped, so the rest has one or two parts, followed by the parts of the variant,
	///     each after an <c>@</c>, which is escaped in the method and the name. When several test files fit, e.g.
	///     <c>AlbumTests.cs</c> and <c>AlbumTests.Tracks.cs</c> for <c>AlbumTests.Tracks.Count</c>, the one for which a
	///     test claimed the snapshot wins, so that a used snapshot is never listed as another, unused one; otherwise the
	///     longest one wins. Without such a test file, which was then deleted or renamed, the last part is taken as the
	///     method.<br />
	///     The id is only used when it is escaped into the same file name again, so that a file that was not written by
	///     the disk store, e.g. one with a name that is not escaped, is never listed.
	/// </remarks>
	private static bool TryGetId(string directory, string baseName, string extension, string[] files,
		[NotNullWhen(true)] out SnapshotId? id)
	{
		List<string> testFiles = files
			.Where(file => ParseFileName(Path.GetFileName(file)) is null)
			.Where(file => baseName.StartsWith(Path.GetFileNameWithoutExtension(file) + ".", PathComparison))
			.OrderByDescending(file => Path.GetFileNameWithoutExtension(file).Length)
			.ToList();
		int variantStart = baseName.IndexOf(SnapshotEnvironment.Separator);
		string withoutVariant = variantStart < 0 ? baseName : baseName.Substring(0, variantStart);
		string testClass = Path.GetFileNameWithoutExtension(withoutVariant);
		if (testFiles.Count == 0 && testClass.Length > 0 && testClass != withoutVariant)
		{
			testFiles.Add(Path.Combine(directory, testClass + ".cs"));
		}

		SnapshotId[] candidates = testFiles
			.Select(testFile => TryGetIdOfTestFile(baseName, extension, testFile))
			.OfType<SnapshotId>()
			.ToArray();
		id = candidates.FirstOrDefault(ClaimedSnapshots.IsClaimed) ?? candidates.FirstOrDefault();
		return id is not null;
	}

	private static SnapshotId? TryGetIdOfTestFile(string baseName, string extension, string testFile)
	{
		string className = baseName.Substring(0, Path.GetFileNameWithoutExtension(testFile).Length);
		string[] variantParts = baseName.Substring(className.Length + 1).Split(SnapshotEnvironment.Separator);
		string[] parts = variantParts[0].Split('.');
		if (parts.Length > 2 || !TryUnescape(parts[0], out string? memberName) || memberName.Length == 0)
		{
			return null;
		}

		string? name = null;
		if (parts.Length == 2 && (!TryUnescape(parts[1], out name) || name.Length == 0))
		{
			return null;
		}

		if (variantParts.Any(part => part.Length == 0))
		{
			return null;
		}

		string? variant = variantParts.Length == 1
			? null
			: string.Join(SnapshotEnvironment.Separator.ToString(), variantParts, 1, variantParts.Length - 1);
		return string.Equals(GetBaseName(className, memberName, name, variant), baseName, StringComparison.Ordinal)
			? new SnapshotId(SourceRoots.GetSourceFileOfPath(testFile), memberName, name)
			{
				Variant = variant,
				Extension = extension,
			}
			: null;
	}

	private static string GetSnapshotPath(SnapshotId id) => GetSnapshotPath(GetBasePath(id), id.Extension);

	private static string GetSnapshotPath(string basePath, string extension)
		=> $"{basePath}{SnapshotSuffix}.{extension}";

	private static string GetPendingPath(SnapshotId id) => GetPendingPath(GetBasePath(id), id.Extension);

	private static string GetPendingPath(string basePath, string extension)
		=> $"{basePath}{PendingSuffix}.{extension}";

	private static string GetBasePath(SnapshotId id)
	{
		string testFilePath = SourceRoots.GetPath(id.SourceFile);
		return Path.Combine(Path.GetDirectoryName(testFilePath) ?? "",
			GetBaseName(Path.GetFileNameWithoutExtension(testFilePath), id.MemberName, id.Name, id.Variant));
	}

	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c> or <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;</c>, followed by
	///     <c>@&lt;Variant&gt;</c> with a <paramref name="variant" />, with the method and the name escaped.
	/// </summary>
	/// <remarks>
	///     The variant is not escaped, as its parts are names like <c>net8.0</c> or <c>x64</c>, which a file name can hold
	///     and which stay readable.
	/// </remarks>
	private static string GetBaseName(string className, string memberName, string? name, string? variant)
	{
		string baseName = name is null
			? $"{className}.{EscapeFileNamePart(memberName)}"
			: $"{className}.{EscapeFileNamePart(memberName)}.{EscapeFileNamePart(name)}";
		return variant is null ? baseName : $"{baseName}{SnapshotEnvironment.Separator}{variant}";
	}

	/// <summary>
	///     Escapes each character as <c>%XX</c> that a file name cannot hold on every operating system
	///     (<c>&lt; &gt; : " / \ | ? *</c> and control characters), as the snapshots are committed and checked out on other
	///     machines, as well as <c>%</c>, <c>.</c> and <c>@</c>.
	/// </summary>
	/// <remarks>
	///     Escaping <c>%</c> keeps the file names of two different names apart, e.g. <c>a:b</c> and <c>a%3Ab</c>, and
	///     escaping <c>.</c> and <c>@</c> keeps them in <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;@&lt;Variant&gt;</c>
	///     as the only separators, so that a file name can be read back into its parts. A name without such characters is
	///     used unchanged.
	/// </remarks>
	private static string EscapeFileNamePart(string part)
	{
		StringBuilder? escaped = null;
		for (int i = 0; i < part.Length; i++)
		{
			char character = part[i];
			if (char.IsControl(character) ||
			    character is '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*' or '%' or '.' or '@')
			{
				escaped ??= new StringBuilder(part, 0, i, part.Length + 8);
				escaped.Append('%').Append(((int)character).ToString("X2", CultureInfo.InvariantCulture));
			}
			else
			{
				escaped?.Append(character);
			}
		}

		return escaped?.ToString() ?? part;
	}

	/// <summary>
	///     Reverses <see cref="EscapeFileNamePart(string)" />, or returns <see langword="false" /> for a <c>%</c> that is
	///     not followed by two hexadecimal digits.
	/// </summary>
	private static bool TryUnescape(string part, [NotNullWhen(true)] out string? unescaped)
	{
		StringBuilder builder = new(part.Length);
		for (int i = 0; i < part.Length; i++)
		{
			if (part[i] != '%')
			{
				builder.Append(part[i]);
				continue;
			}

			if (i + 2 >= part.Length || !Uri.IsHexDigit(part[i + 1]) || !Uri.IsHexDigit(part[i + 2]))
			{
				unescaped = null;
				return false;
			}

			builder.Append((char)((Uri.FromHex(part[i + 1]) << 4) + Uri.FromHex(part[i + 2])));
			i += 2;
		}

		unescaped = builder.ToString();
		return true;
	}

	private static string RemoveFinalLineBreak(string snapshot)
	{
		if (snapshot.EndsWith("\r\n", StringComparison.Ordinal))
		{
			return snapshot.Substring(0, snapshot.Length - 2);
		}

		return snapshot.Length > 0 && snapshot[snapshot.Length - 1] == '\n'
			? snapshot.Substring(0, snapshot.Length - 1)
			: snapshot;
	}
}
