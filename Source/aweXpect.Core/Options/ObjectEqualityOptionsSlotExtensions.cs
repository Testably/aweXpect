using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="ObjectEqualityOptions{TSubject}" /> on the <see cref="OptionsSlot{TOptions}" />
///     of an expectation.
/// </summary>
public static class ObjectEqualityOptionsSlotExtensions
{
	/// <inheritdoc cref="ObjectEqualityOptions{TSubject}.SetMatchType(ObjectMatchType{TSubject}, string)" />
	public static OptionsSlot<ObjectEqualityOptions<TSubject>> SetMatchType<TSubject>(
		this OptionsSlot<ObjectEqualityOptions<TSubject>> slot, ObjectMatchType<TSubject> matchType, string optionName)
	{
		slot.Value = slot.Value.SetMatchType(matchType, optionName);
		return slot;
	}

	/// <inheritdoc cref="ObjectEqualityOptions{TSubject}.Using(IEqualityComparer{object})" />
	public static OptionsSlot<ObjectEqualityOptions<TSubject>> Using<TSubject>(
		this OptionsSlot<ObjectEqualityOptions<TSubject>> slot, IEqualityComparer<object> comparer)
	{
		slot.Value = slot.Value.Using(comparer);
		return slot;
	}

	/// <inheritdoc cref="ObjectEqualityOptions{TSubject}.Using(IEqualityComparer{TSubject})" />
	public static OptionsSlot<ObjectEqualityOptions<TSubject>> Using<TSubject>(
		this OptionsSlot<ObjectEqualityOptions<TSubject>> slot, IEqualityComparer<TSubject> comparer)
	{
		slot.Value = slot.Value.Using(comparer);
		return slot;
	}
}
