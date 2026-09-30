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
	public class EquivalencyCustomization
	{
		private const string KeyPrefix = "aweXpect.Equivalency.";

		internal EquivalencyCustomization(IAwexpectCustomization awexpectCustomization)
		{
			DefaultEquivalencyOptions = new CustomizationValue<EquivalencyOptions>(awexpectCustomization,
				KeyPrefix + nameof(DefaultEquivalencyOptions), new EquivalencyOptions(),
				options => options.ThrowIfNull());
		}

		/// <summary>
		///     The default <see cref="EquivalencyOptions" />.
		/// </summary>
		public ICustomizationValueSetter<EquivalencyOptions> DefaultEquivalencyOptions { get; }
	}
}
