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
	/// </remarks>
	CustomizationLifetime Update(Func<TValue, TValue> update);
}
