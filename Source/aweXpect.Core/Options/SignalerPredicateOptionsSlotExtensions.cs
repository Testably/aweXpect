using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="SignalerPredicateOptions{TParameter}" /> on the
///     <see cref="OptionsSlot{TOptions}" /> of an expectation.
/// </summary>
public static class SignalerPredicateOptionsSlotExtensions
{
	/// <inheritdoc cref="SignalerPredicateOptions{TParameter}.WithPredicate(Func{TParameter, bool}, string)" />
	public static OptionsSlot<SignalerPredicateOptions<TParameter>> WithPredicate<TParameter>(
		this OptionsSlot<SignalerPredicateOptions<TParameter>> slot, Func<TParameter, bool> predicate,
		string predicateExpression)
	{
		slot.Value = slot.Value.WithPredicate(predicate, predicateExpression);
		return slot;
	}
}
