using System;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the message of the thrown exception…
	/// </summary>
	public static PropertyResult.String<Exception?, TException, ThatDelegateThrows<TException>> WithMessage<TException>(
		this ThatDelegateThrows<TException> subject)
		where TException : Exception?
		=> new(subject, e => e?.Message, "Message",
			grammars: ExpectationGrammars.Active | ExpectationGrammars.Nested,
			includeValueInContext: true);

	/// <summary>
	///     Verifies that the thrown exception has a message equal to <paramref name="expected" />.
	/// </summary>
	public static StringEqualityTypeResult<TException, ThatDelegateThrows<TException>> WithMessage<TException>(
		this ThatDelegateThrows<TException> subject,
		string expected)
		where TException : Exception?
		=> subject.WithMessage().EqualTo(expected);
}
