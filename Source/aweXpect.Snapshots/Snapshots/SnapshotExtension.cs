using System;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Checks the file extension of a snapshot, see <see cref="SnapshotId.Extension" />.
/// </summary>
/// <remarks>
///     The extension is not escaped like a name, so it must be valid in a file name on every operating system, and it
///     must not contain a <c>.</c>, so that it is always the last part of a file name and can be read back. <c>%</c>,
///     <c>@</c> and whitespace are not allowed either, as <c>%</c> and <c>@</c> have a meaning in the other parts of a
///     file name, and Windows drops whitespace at the end. <c>pending</c> is not allowed, as
///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending</c> would look like a pending file.
/// </remarks>
internal static class SnapshotExtension
{
	private const string Pending = "pending";

	public static void ThrowIfInvalid(string? extension)
	{
		if (extension is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(extension),
				"The 'extension' cannot be null."));
		}

		string? error = GetError(extension);
		if (error is not null)
		{
			throw Tracing.WriteException(new ArgumentException(error, nameof(extension)));
		}
	}

	public static bool IsValid(string extension) => GetError(extension) is null;

	private static string? GetError(string extension)
	{
		if (string.IsNullOrWhiteSpace(extension))
		{
			return "The 'extension' cannot be empty.";
		}

		if (extension.Contains('.'))
		{
			return "The 'extension' cannot contain a '.', e.g. use \"json\" instead of \".json\".";
		}

		if (extension.Any(IsInvalid))
		{
			return
				"The 'extension' cannot contain whitespace, control characters or any of < > : \" / \\ | ? * % @, as it is part of the file name.";
		}

		return string.Equals(extension, Pending, StringComparison.OrdinalIgnoreCase)
			? "The 'extension' cannot be \"pending\", as it marks the pending file of a snapshot."
			: null;
	}

	private static bool IsInvalid(char character)
		=> char.IsControl(character) || char.IsWhiteSpace(character) ||
		   character is '<' or '>' or ':' or '"' or '/' or '\\' or '|' or '?' or '*' or '%' or '@';
}
