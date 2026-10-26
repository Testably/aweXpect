using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="SignalerPredicateOptions{TParameter}" /> on the
///     <see cref="OptionsHolder{TOptions}" /> of an expectation.
/// </summary>
public static class SignalerPredicateOptionsHolderExtensions
{
	/// <inheritdoc cref="SignalerPredicateOptions{TParameter}.AddPredicate(Func{TParameter, bool}, string)" />
	public static OptionsHolder<SignalerPredicateOptions<TParameter>> AddPredicate<TParameter>(
		this OptionsHolder<SignalerPredicateOptions<TParameter>> holder, Func<TParameter, bool> predicate,
		string predicateExpression)
	{
		holder.Value = holder.Value.AddPredicate(predicate, predicateExpression);
		return holder;
	}
}
