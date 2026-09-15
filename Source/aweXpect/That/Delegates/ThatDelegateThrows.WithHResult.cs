using System;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the actual <see cref="Exception" /> has an <paramref name="expected" /> HResult.
	/// </summary>
	public static AndOrResult<TException, ThatDelegateThrows<TException>> WithHResult<TException>(
		this ThatDelegateThrows<TException> subject,
		int expected)
		where TException : Exception?
		=> new(subject.ExpectationBuilder.AddConstraint((it, grammars)
				=> new ThatException.HasHResultValueConstraint(
					it,
					grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested,
					expected)),
			subject);
}
