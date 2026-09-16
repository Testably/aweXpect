using System;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the HResult of the thrown exception…
	/// </summary>
	public static PropertyResult.Int<Exception?, TException, ThatDelegateThrows<TException>> WithHResult<TException>(
		this ThatDelegateThrows<TException> subject)
		where TException : Exception?
		=> new(subject, e => e?.HResult, "HResult",
			grammars: ExpectationGrammars.Active | ExpectationGrammars.Nested);

	/// <summary>
	///     Verifies that the thrown exception has an HResult equal to <paramref name="expected" />.
	/// </summary>
	public static AndOrResult<TException, ThatDelegateThrows<TException>> WithHResult<TException>(
		this ThatDelegateThrows<TException> subject,
		int expected)
		where TException : Exception?
		=> subject.WithHResult().EqualTo(expected);
}
