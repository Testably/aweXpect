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
	public static PropertyResult.String<Exception?, TException, IThat<TException>> HasMessage<TException>(
		this IThat<TException> subject)
		where TException : Exception?
		=> new(subject, e => e?.Message, "message", includeValueInContext: true);

	/// <summary>
	///     Verifies that the actual exception has a message equal to <paramref name="expected" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TException, IThat<TException>> HasMessage<TException>(
		this IThat<TException> subject,
		string? expected)
		where TException : Exception?
		=> subject.HasMessage().EqualTo(expected);
}
