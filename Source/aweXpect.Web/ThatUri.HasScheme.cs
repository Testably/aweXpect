using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the scheme of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Scheme" />
	/// </remarks>
	public static PropertyResult.String<Uri> HasScheme(this IThat<Uri> source)
		=> new(source, u => u.Scheme, "scheme");
}
