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
	///     If <paramref name="expected" /> is <see langword="null" />, does not verify anything.
	/// </remarks>
	public static AndOrResult<TException, ThatDelegateThrows<TException>> WithParamName<TException>(
		this ThatDelegateThrows<TException> subject,
		string? expected)
		where TException : ArgumentException?
	{
		if (expected == null)
		{
			return new AndOrResult<TException, ThatDelegateThrows<TException>>(subject.ExpectationBuilder, subject);
		}

		return new AndOrResult<TException, ThatDelegateThrows<TException>>(
			subject.ExpectationBuilder.AddConstraint((it, grammars)
				=> new ThatException.HasParamNameValueConstraint<TException>(
					it,
					grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested,
					expected)),
			subject);
	}
}
