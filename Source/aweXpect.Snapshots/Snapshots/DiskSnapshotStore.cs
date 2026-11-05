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
///     Keeps the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.txt</c> next to the test file and
///     records a mismatch in <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.pending.txt</c>, which can be
///     renamed to accept it.
/// </summary>
/// <remarks>
///     The bytes are written and read unchanged, so that a pending file can be renamed to accept it.<br />
///     The files are accessed synchronously, as netstandard2.0 lacks the asynchronous methods of <see cref="File" />.<br />
///     A relative <see cref="SnapshotId.SourceFile" /> is resolved against the recorded source root, see
///     <see cref="SourceRoots" />.<br />
///     The method name is escaped in the file name, see <see cref="EscapeFileNamePart(string)" />, and a
///     <see cref="SnapshotId.Variant" /> is appended after an <c>@</c>, e.g.
///     <c>&lt;Class&gt;.&lt;Method&gt;@net8.0@windows.awexpect-snapshot.txt</c>. The
///     <see cref="SnapshotId.Extension" /> replaces <c>txt</c>, e.g.
///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.pending.json</c>.<br />
///     A snapshot with a <see cref="SnapshotId.Name" /> is kept in a directory for its class and one for its method,
///     <c>&lt;Class&gt;/&lt;Method&gt;/&lt;Name&gt;.awexpect-snapshot.txt</c>, so that the snapshots of a test stay
///     together and their names stay readable, as each part has a path segment of its own and only needs the escaping
///     of <see cref="EscapeNestedPart(string, bool)" />.<br />
///     The distinctive marker <c>.awexpect-snapshot.</c> keeps files of other tools, e.g. <c>*.snapshot.*</c>, from
///     being read or listed as snapshots.
/// </remarks>
internal sealed class DiskSnapshotStore : SnapshotStore, IFileSnapshotStore
{
	public static readonly DiskSnapshotStore Instance = new();
	private const string PendingSuffix = ".awexpect-snapshot.pending";
	private const string SnapshotSuffix = ".awexpect-snapshot";

	private ConcurrentDictionary<SnapshotId, (string BasePath, string Extension)> _listed =
		new(ClaimedSnapshots.Comparer);

	private DiskSnapshotStore()
	{
	}

	/// <remarks>
	///     The test file is resolved before the snapshot is read, so that an ambiguous source root is reported instead of
	///     reading the snapshot next to another test file with the same relative path.
	/// </remarks>
	public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		SourceRoots.ThrowIfUnresolvable(id.SourceFile);
		string snapshotPath = GetSnapshotPath(id);
		if (!File.Exists(snapshotPath))
		{
			return new ValueTask<byte[]?>((byte[]?)null);
		}

		return new ValueTask<byte[]?>(File.ReadAllBytes(snapshotPath));
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		CreateNestedDirectories(id);
		File.WriteAllBytes(GetPendingPath(id), actual);
		return default;
	}

	/// <remarks>
	///     The accepted snapshot makes a leftover pending file obsolete, so one that cannot be deleted does not fail it;
	///     the next matching run deletes it.
	/// </remarks>
	public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		CreateNestedDirectories(id);
		File.WriteAllBytes(GetSnapshotPath(id), actual);
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

	/// <summary>
	///     Creates the directories of the class and the method of a snapshot with a name, but only next to an existing
	///     test file directory, so that a missing one is still reported.
	/// </summary>
	private static void CreateNestedDirectories(SnapshotId id)
	{
		if (id.Name is null)
		{
			return;
		}

		string methodDirectory = Path.GetDirectoryName(GetBasePath(id))!;
		if (Directory.Exists(Path.GetDirectoryName(Path.GetDirectoryName(methodDirectory))))
		{
			Directory.CreateDirectory(methodDirectory);
		}
	}

	/// <summary>
	///     Deletes the directories of the method and the class of a deleted snapshot with a name when they are empty.
	/// </summary>
	/// <remarks>
	///     An empty directory is only a leftover, so one that cannot be deleted does not fail the deletion.
	/// </remarks>
	private static void DeleteEmptyNestedDirectories(string basePath)
	{
		string? directory = Path.GetDirectoryName(basePath);
		for (int i = 0; i < 2 && directory is not null && !Directory.EnumerateFileSystemEntries(directory).Any(); i++)
		{
			try
			{
				Directory.Delete(directory);
			}
			catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
			{
				return;
			}

			directory = Path.GetDirectoryName(directory);
		}
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

	public string GetSnapshotFile(SnapshotId id) => GetSnapshotPath(id);

	public string GetPendingFile(SnapshotId id) => GetPendingPath(id);

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
			CollectSnapshots(projectDirectory, 0, listed, cancellationToken);
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

		if (id.Name is not null)
		{
			DeleteEmptyNestedDirectories(basePath);
		}

		return default;
	}

	/// <remarks>
	///     A snapshot with a name lies two directories below its test file, so from the <paramref name="depth" /> of 2
	///     below the project directory on, the files of that test file directory are passed on as well.
	/// </remarks>
	private static void CollectSnapshots(string directory, int depth,
		ConcurrentDictionary<SnapshotId, (string BasePath, string Extension)> listed,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		string[] files = Directory.GetFiles(directory);
		string[]? nestedTestFiles = depth >= 2
			? Directory.GetFiles(Path.GetDirectoryName(Path.GetDirectoryName(directory))!)
			: null;
		foreach (string file in files)
		{
			(string BaseName, string Extension)? snapshotFile = ParseFileName(Path.GetFileName(file));
			if (snapshotFile is { } parsed &&
			    TryGetId(directory, parsed, files, nestedTestFiles, out SnapshotId? id))
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

			CollectSnapshots(subdirectory, depth + 1, listed, cancellationToken);
		}
	}

	/// <summary>
	///     Splits the name of a snapshot or pending file into the base name and the extension, or returns
	///     <see langword="null" /> for any other file.
	/// </summary>
	/// <remarks>
	///     The extension is the part after the last <c>.</c>, as it cannot contain one, so
	///     <c>&lt;Base&gt;.awexpect-snapshot.pending.json</c> is always the pending file of
	///     <c>&lt;Base&gt;.awexpect-snapshot.json</c>, never a snapshot with the extension <c>pending.json</c>. An
	///     extension that <c>.WithExtension(…)</c> does not allow, e.g. <c>pending</c>, is no snapshot file, nor is a
	///     file without the marker, e.g. <c>&lt;Base&gt;.snapshot.txt</c>.
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
	///     Reads the base name of a snapshot file in the <paramref name="directory" /> back into the id that it was
	///     written for.
	/// </summary>
	/// <remarks>
	///     A snapshot without a name lies next to its test file, see <see cref="GetFlatIds" />, and one with a name two
	///     directories below it, see <see cref="GetNestedIds" />, whose test file is one of the
	///     <paramref name="nestedTestFiles" />. When several test files fit, the one for which a test claimed the snapshot
	///     wins, so that a used snapshot is never listed as another, unused one; otherwise the first one wins. Only without
	///     any test file that fits, which was then deleted or renamed, the test file is guessed, see
	///     <see cref="GetIdsOfDeletedTestFile" />.<br />
	///     The id is only used when it is escaped into the same path again, so that a file that was not written by the
	///     disk store, e.g. one with a name that is not escaped, is never listed.
	/// </remarks>
	private static bool TryGetId(string directory, (string BaseName, string Extension) file, string[] files,
		string[]? nestedTestFiles, [NotNullWhen(true)] out SnapshotId? id)
	{
		string[] flatTestFiles = files
			.Where(testFile => ParseFileName(Path.GetFileName(testFile)) is null)
			.Where(testFile => file.BaseName.StartsWith(Path.GetFileNameWithoutExtension(testFile) + ".",
				FileSystemPaths.Comparison))
			.OrderByDescending(testFile => Path.GetFileNameWithoutExtension(testFile).Length)
			.ToArray();
		string className = Path.GetFileName(Path.GetDirectoryName(directory)) ?? "";
		string[] nestedFiles = nestedTestFiles?
			.Where(testFile => ParseFileName(Path.GetFileName(testFile)) is null)
			.Where(testFile => string.Equals(Path.GetFileNameWithoutExtension(testFile), className,
				FileSystemPaths.Comparison))
			.ToArray() ?? [];
		List<SnapshotId> candidates = flatTestFiles.Length == 0 && nestedFiles.Length == 0
			? GetIdsOfDeletedTestFile(directory, file.BaseName, file.Extension, nestedTestFiles is not null)
			:
			[
				.. GetFlatIds(file.BaseName, file.Extension, flatTestFiles),
				.. GetNestedIds(directory, file.BaseName, file.Extension, nestedFiles),
			];

		id = candidates.FirstOrDefault(ClaimedSnapshots.IsClaimed) ?? candidates.FirstOrDefault();
		return id is not null;
	}

	/// <summary>
	///     Guesses the test file of a snapshot whose test file was deleted or renamed: <c>&lt;Class&gt;.cs</c> after the
	///     directory of the class when the snapshot can have a name, otherwise after the part before the last <c>.</c>.
	/// </summary>
	private static List<SnapshotId> GetIdsOfDeletedTestFile(string directory, string baseName, string extension,
		bool canBeNested)
	{
		if (canBeNested)
		{
			string classDirectory = Path.GetDirectoryName(directory)!;
			List<SnapshotId> nestedIds = GetNestedIds(directory, baseName, extension,
				[Path.Combine(Path.GetDirectoryName(classDirectory)!, Path.GetFileName(classDirectory) + ".cs"),]);
			if (nestedIds.Count > 0)
			{
				return nestedIds;
			}
		}

		int variantStart = baseName.IndexOf(SnapshotEnvironment.Separator);
		string withoutVariant = variantStart < 0 ? baseName : baseName.Substring(0, variantStart);
		string testClass = Path.GetFileNameWithoutExtension(withoutVariant);
		return testClass.Length > 0 && testClass != withoutVariant
			? GetFlatIds(baseName, extension, [Path.Combine(directory, testClass + ".cs"),])
			: [];
	}

	/// <summary>
	///     Reads <c>&lt;Class&gt;.&lt;Method&gt;</c> back, where the class part is the name of the test file, which may
	///     contain dots, and the method follows with its dots escaped, followed by the parts of the variant, each after an
	///     <c>@</c>, which is escaped in the method.
	/// </summary>
	private static List<SnapshotId> GetFlatIds(string baseName, string extension, IEnumerable<string> testFiles)
	{
		List<SnapshotId> ids = [];
		foreach (string testFile in testFiles)
		{
			string className = baseName.Substring(0, Path.GetFileNameWithoutExtension(testFile).Length);
			string[] variantParts = baseName.Substring(className.Length + 1).Split(SnapshotEnvironment.Separator);
			if (TryUnescape(variantParts[0], out string? memberName) && memberName.Length > 0 &&
			    TryGetVariant(variantParts, out string? variant) &&
			    string.Equals(GetFlatBaseName(className, memberName, variant), baseName, StringComparison.Ordinal))
			{
				ids.Add(new SnapshotId(SourceRoots.GetSourceFileOfPath(testFile), memberName)
				{
					Variant = variant,
					Extension = extension,
				});
			}
		}

		return ids;
	}

	/// <summary>
	///     Reads <c>&lt;Class&gt;/&lt;Method&gt;/&lt;Name&gt;</c> back, where the <paramref name="directory" /> is the one
	///     of the method and the <paramref name="baseName" /> is the name, followed by the parts of the variant, each after
	///     an <c>@</c>, which is escaped in the name.
	/// </summary>
	private static List<SnapshotId> GetNestedIds(string directory, string baseName, string extension,
		IEnumerable<string> testFiles)
	{
		string methodDirectory = Path.GetFileName(directory);
		string[] variantParts = baseName.Split(SnapshotEnvironment.Separator);
		if (!TryUnescape(methodDirectory, out string? memberName) || memberName.Length == 0 ||
		    !TryUnescape(variantParts[0], out string? name) || name.Length == 0 ||
		    !TryGetVariant(variantParts, out string? variant) ||
		    !string.Equals(EscapeNestedPart(memberName, true), methodDirectory, StringComparison.Ordinal) ||
		    !string.Equals(GetNestedBaseName(name, variant), baseName, StringComparison.Ordinal))
		{
			return [];
		}

		return testFiles
			.Select(testFile => new SnapshotId(SourceRoots.GetSourceFileOfPath(testFile), memberName, name)
			{
				Variant = variant,
				Extension = extension,
			})
			.ToList();
	}

	/// <summary>
	///     Joins the parts of the variant after the first of the <paramref name="variantParts" />, or returns
	///     <see langword="false" /> when one of them is empty.
	/// </summary>
	private static bool TryGetVariant(string[] variantParts, out string? variant)
	{
		variant = variantParts.Length == 1
			? null
			: string.Join(SnapshotEnvironment.Separator.ToString(), variantParts, 1, variantParts.Length - 1);
		return variantParts.All(part => part.Length > 0);
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
		string directory = Path.GetDirectoryName(testFilePath) ?? "";
		string className = Path.GetFileNameWithoutExtension(testFilePath);
		return id.Name is null
			? Path.Combine(directory, GetFlatBaseName(className, id.MemberName, id.Variant))
			: Path.Combine(directory, className, EscapeNestedPart(id.MemberName, true),
				GetNestedBaseName(id.Name, id.Variant));
	}

	/// <summary>
	///     Returns <c>&lt;Class&gt;.&lt;Method&gt;</c>, followed by <c>@&lt;Variant&gt;</c> with a
	///     <paramref name="variant" />, with the method escaped.
	/// </summary>
	/// <remarks>
	///     The variant is not escaped, as its parts are names like <c>net8.0</c> or <c>x64</c>, which a file name can hold
	///     and which stay readable.
	/// </remarks>
	private static string GetFlatBaseName(string className, string memberName, string? variant)
	{
		string baseName = $"{className}.{EscapeFileNamePart(memberName)}";
		return variant is null ? baseName : $"{baseName}{SnapshotEnvironment.Separator}{variant}";
	}

	/// <summary>
	///     Returns the escaped <paramref name="name" />, followed by <c>@&lt;Variant&gt;</c> with a
	///     <paramref name="variant" />.
	/// </summary>
	private static string GetNestedBaseName(string name, string? variant)
	{
		string baseName = EscapeNestedPart(name, false);
		return variant is null ? baseName : $"{baseName}{SnapshotEnvironment.Separator}{variant}";
	}

	/// <summary>
	///     Escapes each character as <c>%XX</c> that a file name cannot hold on every operating system
	///     (<c>&lt; &gt; : " / \ | ? *</c> and control characters), as the snapshots are committed and checked out on other
	///     machines, as well as <c>%</c>, <c>.</c> and <c>@</c>.
	/// </summary>
	/// <remarks>
	///     Escaping <c>%</c> keeps the file names of two different names apart, e.g. <c>a:b</c> and <c>a%3Ab</c>, and
	///     escaping <c>.</c> and <c>@</c> keeps them in <c>&lt;Class&gt;.&lt;Method&gt;@&lt;Variant&gt;</c> as the only
	///     separators, so that a file name can be read back into its parts. A name without such characters is used
	///     unchanged.
	/// </remarks>
	private static string EscapeFileNamePart(string part)
		=> Escape(part, static (character, _) => IsEscaped(character));

	/// <summary>
	///     Escapes the method or the name of a snapshot with a name as a path segment of its own, like
	///     <see cref="EscapeFileNamePart(string)" />, but keeps <c>.</c>, as the segments are separated by the directories.
	/// </summary>
	/// <remarks>
	///     The first character of a name that Windows reserves for a device, e.g. <c>CON</c> or <c>com1.json</c>, is
	///     escaped, as is a final <c>.</c> or space of a <paramref name="isDirectory">directory</paramref>, which Windows
	///     removes, while a file always ends with the marker.
	/// </remarks>
	private static string EscapeNestedPart(string part, bool isDirectory)
	{
		bool escapesFirst = IsReservedName(part);
		bool escapesLast = isDirectory && part.Length > 0 && part[part.Length - 1] is '.' or ' ';
		return Escape(part, (character, index)
			=> IsEscapedInNestedPart(character) ||
			   (index == 0 && escapesFirst) ||
			   (index == part.Length - 1 && escapesLast));
	}

	private static bool IsReservedName(string part)
	{
		int dot = part.IndexOf('.');
		string stem = (dot < 0 ? part : part.Substring(0, dot)).TrimEnd(' ').ToUpperInvariant();
		return stem is "CON" or "PRN" or "AUX" or "NUL" ||
		       (stem.Length == 4 && stem.StartsWith("COM", StringComparison.Ordinal) && char.IsDigit(stem[3])) ||
		       (stem.Length == 4 && stem.StartsWith("LPT", StringComparison.Ordinal) && char.IsDigit(stem[3]));
	}

	private static string Escape(string part, Func<char, int, bool> isEscaped)
	{
		StringBuilder? escaped = null;
		for (int i = 0; i < part.Length; i++)
		{
			char character = part[i];
			if (isEscaped(character, i))
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
	///     Whether <see cref="EscapeFileNamePart(string)" /> escapes the <paramref name="character" /> as <c>%XX</c>.
	/// </summary>
	private static bool IsEscaped(char character)
		=> character == '.' || IsEscapedInNestedPart(character);

	/// <summary>
	///     Whether <see cref="EscapeNestedPart(string, bool)" /> escapes the <paramref name="character" /> as <c>%XX</c>
	///     wherever it is.
	/// </summary>
	internal static bool IsEscapedInNestedPart(char character)
		=> char.IsControl(character) ||
		   character is '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*' or '%' or '@';

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
}
