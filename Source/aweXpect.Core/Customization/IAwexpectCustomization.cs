namespace aweXpect.Customization;

/// <summary>
///     Customize the global behaviour of aweXpect.
/// </summary>
public interface IAwexpectCustomization
{
	/// <summary>
	///     Get the customization <typeparamref name="TValue" /> stored under the given <paramref name="key" />.
	/// </summary>
	/// <remarks>
	///     If no customization was stored, use the given <paramref name="defaultValue" />.
	/// </remarks>
	TValue Get<TValue>(string key, TValue defaultValue);

	/// <summary>
	///     Set the customization <typeparamref name="TValue" /> stored under the given <paramref name="key" />.
	/// </summary>
	/// <remarks>
	///     When the returned <see cref="CustomizationLifetime" /> is disposed, the value is removed again, so that the
	///     previous value applies, unless a value that was set afterwards is still active.
	///     <para />
	///     The value applies to the current async flow and the flows started from it afterwards, or to all async flows
	///     when set on <see cref="AwexpectCustomization.Global" />.
	/// </remarks>
	CustomizationLifetime Set<TValue>(string key, TValue value);
}
