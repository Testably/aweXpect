using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the message of the thrown exception…
	/// </summary>
	public static PropertyResult.String<Exception?, TException, IThatDelegateThrows<TException>> WithMessage<TException>(
		this IThatDelegateThrows<TException> subject)
		where TException : Exception?
		=> new(subject, e => e?.Message, "message",
			grammars: ExpectationGrammars.Active | ExpectationGrammars.Nested,
			includeValueInContext: true);

	/// <summary>
	///     Verifies that the thrown exception has a message equal to <paramref name="expected" />.
	/// </summary>
	public static StringEqualityTypeResult<TException, IThatDelegateThrows<TException>> WithMessage<TException>(
		this IThatDelegateThrows<TException> subject,
		string? expected)
		where TException : Exception?
		=> subject.WithMessage().EqualTo(expected);
}
