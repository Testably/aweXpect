using aweXpect.Core.Helpers;
using aweXpect.Equivalency;

namespace aweXpect.Customization;

public partial class AwexpectCustomization
{
	private EquivalencyCustomization? _equivalency;

	/// <summary>
	///     Customize the equivalency settings.
	/// </summary>
	public EquivalencyCustomization Equivalency()
	{
		_equivalency ??= new EquivalencyCustomization(this);
		return _equivalency;
	}

	/// <summary>
	///     Customize the equivalency settings.
	/// </summary>
	public class EquivalencyCustomization : IAwexpectCustomization
	{
		private const string KeyPrefix = "aweXpect.Equivalency.";
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal EquivalencyCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			DefaultEquivalencyOptions = new CustomizationValue<EquivalencyOptions>(awexpectCustomization,
				KeyPrefix + nameof(DefaultEquivalencyOptions), new EquivalencyOptions(),
				options => options.ThrowIfNull());
		}

		/// <summary>
		///     The default <see cref="EquivalencyOptions" />.
		/// </summary>
		public ICustomizationValueSetter<EquivalencyOptions> DefaultEquivalencyOptions { get; }

		/// <inheritdoc cref="IAwexpectCustomization.Get{TValue}(string, TValue)" />
		TValue IAwexpectCustomization.Get<TValue>(string key, TValue defaultValue)
			=> _awexpectCustomization.Get(key, defaultValue);

		/// <inheritdoc cref="IAwexpectCustomization.Set{TValue}(string, TValue)" />
		CustomizationLifetime IAwexpectCustomization.Set<TValue>(string key, TValue value)
			=> _awexpectCustomization.Set(key, value);
	}
}
