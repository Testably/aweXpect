using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the URL-decoded path of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     An encoded slash (<c>%2F</c>) stays encoded, as it is part of a segment and not a separator.
	///     <br />
	///     <seealso cref="Uri.AbsolutePath" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri, IThat<Uri?>, Uri?> HasPath(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u is null ? null : DecodePath(u.AbsolutePath), "path");

	/// <summary>
	///     Verifies that the URL-decoded path of the <see cref="Uri" /> subject is equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     An encoded slash (<c>%2F</c>) stays encoded, as it is part of a segment and not a separator.
	///     <br />
	///     <seealso cref="Uri.AbsolutePath" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasPath(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasPath().EqualTo(expected);
	}

	// .NET Framework keeps more of Uri.AbsolutePath escaped than .NET, including the case of %2f, so everything except
	// %2F is decoded here to give the same path on every target framework.
	private static string DecodePath(string path)
	{
		int index = path.IndexOf("%2F", StringComparison.OrdinalIgnoreCase);
		if (index < 0)
		{
			return Uri.UnescapeDataString(path);
		}

		StringBuilder sb = new();
		int start = 0;
		while (index >= 0)
		{
			sb.Append(Uri.UnescapeDataString(path.Substring(start, index - start))).Append("%2F");
			start = index + 3;
			index = path.IndexOf("%2F", start, StringComparison.OrdinalIgnoreCase);
		}

		return sb.Append(Uri.UnescapeDataString(path.Substring(start))).ToString();
	}
}
