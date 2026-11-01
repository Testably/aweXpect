using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the host of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     The host is compared ignoring case by default, as RFC 3986 (section 3.2.2) defines it case-insensitively;
	///     <c>IgnoringCase(false)</c> compares it case-sensitively.
	///     <br />
	///     <seealso cref="Uri.Host" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri, IThat<Uri?>, Uri?> HasHost(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u?.Host, "host", ignoringCase: true);

	/// <summary>
	///     Verifies that the host of the <see cref="Uri" /> subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The host is compared ignoring case by default, as RFC 3986 (section 3.2.2) defines it case-insensitively;
	///     <c>IgnoringCase(false)</c> compares it case-sensitively.
	///     <br />
	///     <seealso cref="Uri.Host" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasHost(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasHost().EqualTo(expected);
	}
}
