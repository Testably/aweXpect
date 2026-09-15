using System;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the param name of the thrown <see cref="ArgumentException" />…
	/// </summary>
	public static PropertyResult.String<Exception?, TException, ThatDelegateThrows<TException>>
		WithParamName<TException>(
			this ThatDelegateThrows<TException> subject)
		where TException : ArgumentException?
		=> new(subject, e => (e as ArgumentException)?.ParamName, "ParamName",
			grammars: ExpectationGrammars.Active | ExpectationGrammars.Nested);

	/// <summary>
	///     Verifies that the actual <see cref="ArgumentException" /> has an <paramref name="expected" /> param name.
	/// </summary>
	/// <remarks>
	///     Shorthand for <c>WithParamName().EqualTo(expected)</c>, so a <see langword="null" />
	///     <paramref name="expected" /> requires the param name to be <see langword="null" /> as well.
	/// </remarks>
	public static StringEqualityTypeResult<TException, ThatDelegateThrows<TException>> WithParamName<TException>(
		this ThatDelegateThrows<TException> subject,
		string? expected)
		where TException : ArgumentException?
		=> subject.WithParamName().EqualTo(expected);
}
