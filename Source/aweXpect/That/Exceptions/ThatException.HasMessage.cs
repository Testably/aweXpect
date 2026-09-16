using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatException
{
	/// <summary>
	///     Verifies that the message of the actual exception…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.String<Exception?, Exception?, IThat<Exception?>> HasMessage(
		this IThat<Exception?> subject)
		=> new(subject, e => e?.Message, "Message", includeValueInContext: true);

	/// <summary>
	///     Verifies that the actual exception has a message equal to <paramref name="expected" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Exception?, IThat<Exception?>> HasMessage(
		this IThat<Exception?> subject,
		string expected)
		=> subject.HasMessage().EqualTo(expected);
}
