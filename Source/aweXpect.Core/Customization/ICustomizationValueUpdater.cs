using System;

namespace aweXpect.Customization;

/// <summary>
///     A customization value of type <typeparamref name="TValue" /> that can be updated.
/// </summary>
/// <remarks>
///     This is primarily intended for record types.
/// </remarks>
public interface ICustomizationValueUpdater<TValue>
{
	/// <summary>
	///     Get the stored <typeparamref name="TValue" />.
	/// </summary>
	TValue Get();

	/// <summary>
	///     Update the stored <typeparamref name="TValue" />.
	/// </summary>
	/// <remarks>
	///     Disposing the returned <see cref="CustomizationLifetime" /> restores the whole <typeparamref name="TValue" />
	///     as it was before the update.
	///     <para />
	///     The <paramref name="update" /> function can run again later, e.g. when a lifetime of the same group that was
	///     created before is disposed first, so it must compute the new value only from its argument and must not have
	///     side effects.
	/// </remarks>
	CustomizationLifetime Update(Func<TValue, TValue> update);
}
