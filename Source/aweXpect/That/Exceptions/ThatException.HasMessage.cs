using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
public static partial class ThatException
{
	/// <summary>
	///     Verifies that the message of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.String<Exception?, TException, IThat<TException?>> HasMessage<TException>(
		this IThat<TException?> subject)
		where TException : Exception
		=> new(subject, e => e?.Message, "message", includeValueInContext: true);

	/// <summary>
	///     Verifies that the subject has a message equal to <paramref name="expected" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TException, IThat<TException?>> HasMessage<TException>(
		this IThat<TException?> subject,
		string? expected)
		where TException : Exception
		=> subject.HasMessage().EqualTo(expected);
}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
