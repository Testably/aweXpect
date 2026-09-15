using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Delegates;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the thrown exception has a message equal to <paramref name="expected" />.
	/// </summary>
	public static StringEqualityTypeResult<TException, ThatDelegateThrows<TException>> WithMessage<TException>(
		this ThatDelegateThrows<TException> subject,
		string expected)
		where TException : Exception?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TException, ThatDelegateThrows<TException>>(
			subject.ExpectationBuilder.AddConstraint((it, grammars)
				=> new ThatException.HasMessageValueConstraint(
					subject.ExpectationBuilder,
					it,
					grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested,
					expected,
					options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the thrown exception does not have a message equal to <paramref name="unexpected" />.
	/// </summary>
	public static StringEqualityTypeResult<TException, ThatDelegateThrows<TException>> WithoutMessage<TException>(
		this ThatDelegateThrows<TException> subject,
		string unexpected)
		where TException : Exception?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<TException, ThatDelegateThrows<TException>>(
			subject.ExpectationBuilder.AddConstraint((it, grammars)
				=> new ThatException.HasMessageValueConstraint(
					subject.ExpectationBuilder,
					it,
					grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested,
					unexpected,
					options).Invert()),
			subject,
			options);
	}
}
