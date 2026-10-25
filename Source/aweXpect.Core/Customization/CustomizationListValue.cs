using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.Helpers;

namespace aweXpect.Customization;

/// <summary>
///     A customization value of a list of <typeparamref name="TItem" />, which is stored as a read-only copy under the
///     <c>key</c> in the <see cref="IAwexpectCustomization" />.
/// </summary>
/// <remarks>
///     Use it instead of a <see cref="CustomizationValue{TValue}" /> of an <see cref="IReadOnlyList{T}" />: a stored
///     list would otherwise change with a mutable list it was set from or that was cast from the returned list, which
///     would bypass the scoping of the setting.
///     <para />
///     The instance holds no value itself, so it is safe to share and to use concurrently.
/// </remarks>
public sealed class CustomizationListValue<TItem> : ICustomizationValueSetter<IReadOnlyList<TItem>>
{
	private readonly CustomizationValue<IReadOnlyList<TItem>> _inner;

	/// <summary>
	///     Creates a customization value of a list that is stored under the <paramref name="key" /> in the
	///     <paramref name="customization" />.
	/// </summary>
	/// <param name="customization">The customization that stores the value.</param>
	/// <param name="key">The key under which the value is stored.</param>
	/// <param name="defaultValue">
	///     The list that <see cref="Get()" /> returns while no value is set. It is returned as it is, so pass a
	///     read-only list, e.g. from <see cref="Array.AsReadOnly{T}(T[])" />.
	/// </param>
	/// <param name="validate">
	///     Checks the copy of a list before <see cref="Set(IReadOnlyList{TItem})" /> stores it, and throws for an invalid
	///     list, e.g. an <see cref="ArgumentException" /> for an invalid item.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///     The <paramref name="customization" /> or the <paramref name="key" /> is <see langword="null" />.
	/// </exception>
	public CustomizationListValue(
		IAwexpectCustomization customization,
		string key,
		IReadOnlyList<TItem> defaultValue,
		Action<IReadOnlyList<TItem>>? validate = null)
	{
		_inner = new CustomizationValue<IReadOnlyList<TItem>>(customization, key, defaultValue, validate);
	}

	/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
	public IReadOnlyList<TItem> Get() => _inner.Get();

	/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
	/// <remarks>
	///     Stores a read-only copy of the <paramref name="value" />, so changing the list afterwards does not change the
	///     setting. The copy is validated before it is stored, so an invalid list throws and changes nothing. Dispose the
	///     returned <see cref="CustomizationLifetime" /> to restore the previous value.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
	public CustomizationLifetime Set(IReadOnlyList<TItem> value)
	{
		value.ThrowIfNull();
		return _inner.Set(Array.AsReadOnly(value.ToArray()));
	}
}
