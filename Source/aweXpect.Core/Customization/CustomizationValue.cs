using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Customization;

/// <summary>
///     A customization value of type <typeparamref name="TValue" />, which is stored under the <c>key</c> in the
///     <see cref="IAwexpectCustomization" />.
/// </summary>
/// <remarks>
///     Return it from an extension method on <see cref="AwexpectCustomization" /> or on one of its groups, e.g.
///     <see cref="AwexpectCustomization.ReflectionCustomization" />, so that users can customize a value of your extension
///     like the built-in ones, also for all async flows with <see cref="AwexpectCustomization.Global" />.
///     <para />
///     The instance holds no value itself, so it is safe to share and to use concurrently. Choose a key that no other
///     package uses, e.g. one prefixed with the name of your package.
/// </remarks>
public sealed class CustomizationValue<TValue> : ICustomizationValueSetter<TValue>
{
	private readonly IAwexpectCustomization _customization;
	private readonly TValue _defaultValue;
	private readonly string _key;
	private readonly Action<TValue>? _validate;

	/// <summary>
	///     Creates a customization value that is stored under the <paramref name="key" /> in the
	///     <paramref name="customization" />.
	/// </summary>
	/// <param name="customization">The customization that stores the value.</param>
	/// <param name="key">The key under which the value is stored.</param>
	/// <param name="defaultValue">
	///     The value that <see cref="Get()" /> returns while no value is set. It is returned as it is, so a mutable
	///     default is shared by all callers.
	/// </param>
	/// <param name="validate">
	///     Checks a value before <see cref="Set(TValue)" /> stores it, and throws for an invalid value, e.g. an
	///     <see cref="ArgumentOutOfRangeException" />.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///     The <paramref name="customization" /> or the <paramref name="key" /> is <see langword="null" />.
	/// </exception>
	public CustomizationValue(
		IAwexpectCustomization customization,
		string key,
		TValue defaultValue,
		Action<TValue>? validate = null)
	{
		customization.ThrowIfNull();
		key.ThrowIfNull();
		_customization = customization;
		_key = key;
		_defaultValue = defaultValue;
		_validate = validate;
	}

	/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
	/// <remarks>
	///     A <see langword="null" /> that was set is returned as <see langword="null" />, not as the default value.
	/// </remarks>
	public TValue Get() => _customization.Get(_key, _defaultValue);

	/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
	/// <remarks>
	///     The value is validated before it is stored, so an invalid value throws and changes nothing. Dispose the
	///     returned <see cref="CustomizationLifetime" /> to restore the previous value.
	/// </remarks>
	public CustomizationLifetime Set(TValue value)
	{
		_validate?.Invoke(value);
		return _customization.Set(_key, value);
	}
}
