using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Delegates;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDelegateThrows
{
	/// <summary>
	///     Verifies that the actual exception recursively has inner exceptions which satisfy the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     Recursively applies the expectations on the <see cref="Exception.InnerException" /> (if not <see langword="null" />
	///     and for <see cref="AggregateException" /> also on the <see cref="AggregateException.InnerExceptions" />.
	///     <para />
	///     An exception without any inner exception fails a quantifier that an empty collection satisfies without
	///     stating anything (<c>All()</c>), but still satisfies the quantifiers that state an upper bound
	///     (e.g. <c>None()</c> or <c>AtMost(2)</c>).
	/// </remarks>
	public static AndOrResult<TException?, ThatDelegateThrows<TException>> WithRecursiveInnerExceptions<TException>(
		this ThatDelegateThrows<TException> subject,
		Action<IThatSubject<IEnumerable<Exception>>> expectations)
		where TException : Exception?
		=> new(subject.ExpectationBuilder
				.ForMember(
					MemberAccessor<Exception?, IEnumerable<Exception>>.FromFunc(
						e => e.GetInnerExceptions(),
						"recursive inner exceptions"),
					(_, s) => s.Append(" which "))
				.Validate((it, grammars) => new ThatException.HasRecursiveInnerExceptionsConstraint(
					it, grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested))
				.AddExpectations(e => expectations(new ThatSubject<IEnumerable<Exception>>(e)),
					grammars => grammars | ExpectationGrammars.Active | ExpectationGrammars.Nested |
					            ExpectationGrammars.Plural),
			subject);
}
