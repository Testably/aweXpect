using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="ObjectEqualityWithToleranceOptions{TSubject, TTolerance}" /> on the
///     <see cref="OptionsSlot{TOptions}" /> of an expectation.
/// </summary>
public static class ObjectEqualityWithToleranceOptionsSlotExtensions
{
	/// <inheritdoc cref="ObjectEqualityWithToleranceOptions{TSubject, TTolerance}.Within(TTolerance)" />
	public static OptionsSlot<ObjectEqualityWithToleranceOptions<TSubject, TTolerance>> Within<TSubject, TTolerance>(
		this OptionsSlot<ObjectEqualityWithToleranceOptions<TSubject, TTolerance>> slot, TTolerance tolerance)
	{
		slot.Value = slot.Value.Within(tolerance);
		return slot;
	}
}
