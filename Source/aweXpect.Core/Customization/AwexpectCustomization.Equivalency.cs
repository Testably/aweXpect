using System;
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
	public class EquivalencyCustomization : ICustomizationValueUpdater<EquivalencyCustomizationValue>
	{
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal EquivalencyCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			DefaultEquivalencyOptions = new CustomizationValue<EquivalencyCustomizationValue, EquivalencyOptions>(this,
				p => p.DefaultEquivalencyOptions,
				(p, v) => p with
				{
					DefaultEquivalencyOptions = v,
				});
		}

		/// <inheritdoc cref="EquivalencyCustomizationValue.DefaultEquivalencyOptions" />
		public ICustomizationValueSetter<EquivalencyOptions> DefaultEquivalencyOptions { get; }

		/// <inheritdoc cref="ICustomizationValueUpdater{EquivalencyCustomizationValue}.Get()" />
		public EquivalencyCustomizationValue Get()
			=> _awexpectCustomization.Get(nameof(Equivalency), new EquivalencyCustomizationValue());

		/// <inheritdoc
		///     cref="ICustomizationValueUpdater{EquivalencyCustomizationValue}.Update(Func{EquivalencyCustomizationValue,EquivalencyCustomizationValue})" />
		public CustomizationLifetime Update(Func<EquivalencyCustomizationValue, EquivalencyCustomizationValue> update)
			=> _awexpectCustomization.Set(nameof(Equivalency), update(Get()));
	}

	/// <summary>
	///     Customize the equivalency settings.
	/// </summary>
	public record EquivalencyCustomizationValue
	{
		/// <summary>
		///     The default <see cref="EquivalencyOptions" />.
		/// </summary>
		public EquivalencyOptions DefaultEquivalencyOptions { get; init; } = new();
	}
}
