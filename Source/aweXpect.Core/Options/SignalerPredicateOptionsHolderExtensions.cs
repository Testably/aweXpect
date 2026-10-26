using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="SignalerPredicateOptions{TParameter}" /> on the
///     <see cref="OptionsHolder{TOptions}" /> of an expectation.
/// </summary>
public static class SignalerPredicateOptionsHolderExtensions
{
	/// <inheritdoc cref="SignalerPredicateOptions{TParameter}.WithPredicate(Func{TParameter, bool}, string)" />
	public static OptionsHolder<SignalerPredicateOptions<TParameter>> WithPredicate<TParameter>(
		this OptionsHolder<SignalerPredicateOptions<TParameter>> holder, Func<TParameter, bool> predicate,
		string predicateExpression)
	{
		holder.Value = holder.Value.WithPredicate(predicate, predicateExpression);
		return holder;
	}
}
