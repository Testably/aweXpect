using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the param name of the thrown <see cref="ArgumentException" />…
	/// </summary>
	public static PropertyResult.String<Exception?, TException, IThatDelegateThrows<TException>>
		WithParamName<TException>(
			this IThatDelegateThrows<TException> subject)
		where TException : ArgumentException?
		=> new(subject, e => (e as ArgumentException)?.ParamName, "param name",
			grammars: ExpectationGrammars.Active | ExpectationGrammars.Nested,
			includeValueInContext: true);

	/// <summary>
	///     Verifies that the thrown <see cref="ArgumentException" /> has an <paramref name="expected" /> param name.
	/// </summary>
	/// <remarks>
	///     Shorthand for <c>WithParamName().EqualTo(expected)</c>, so a <see langword="null" />
	///     <paramref name="expected" /> requires the param name to be <see langword="null" /> as well.
	/// </remarks>
	public static StringEqualityTypeResult<TException, IThatDelegateThrows<TException>> WithParamName<TException>(
		this IThatDelegateThrows<TException> subject,
		string? expected)
		where TException : ArgumentException?
		=> subject.WithParamName().EqualTo(expected);
}
