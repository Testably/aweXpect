using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the port of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Port" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Uri?, Uri, IThat<Uri?>> HasPort(this IThat<Uri?> source)
		=> new(source.FailIfRelative(), u => u?.Port, "port");

	/// <summary>
	///     Verifies that the port of the <see cref="Uri" /> subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Port" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Uri, IThat<Uri?>> HasPort(this IThat<Uri?> source, int expected)
		=> source.HasPort().EqualTo(expected);
}
