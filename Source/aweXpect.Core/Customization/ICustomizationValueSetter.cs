namespace aweXpect.Customization;

/// <summary>
///     A customization value of type <typeparamref name="TValue" /> that can be set.
/// </summary>
/// <remarks>
///     This is primarily intended for primitive types.
/// </remarks>
public interface ICustomizationValueSetter<TValue>
{
	/// <summary>
	///     Get the stored <typeparamref name="TValue" />.
	/// </summary>
	TValue Get();

	/// <summary>
	///     Set the stored <typeparamref name="TValue" />.
	/// </summary>
	CustomizationLifetime Set(TValue value);
}
