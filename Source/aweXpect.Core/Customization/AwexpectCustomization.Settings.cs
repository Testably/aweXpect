using System;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Customization;

public partial class AwexpectCustomization
{
	private SettingsCustomization? _settings;

	/// <summary>
	///     Customize the settings.
	/// </summary>
	public SettingsCustomization Settings()
	{
		_settings ??= new SettingsCustomization(this);
		return _settings;
	}

	/// <summary>
	///     Customize the settings.
	/// </summary>
	public class SettingsCustomization : ICustomizationValueUpdater<SettingsCustomizationValue>
	{
		private static readonly SettingsCustomizationValue EmptyValue = new();
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal SettingsCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			DefaultCheckInterval = new CustomizationValue<TimeSpan>(
				() => Get().DefaultCheckInterval,
				v => Update(p => p with
				{
					DefaultCheckInterval = v,
				}));
			DefaultEventuallyTimeout = new CustomizationValue<TimeSpan>(
				() => Get().DefaultEventuallyTimeout,
				v => Update(p => p with
				{
					DefaultEventuallyTimeout = v,
				}));
			DefaultSignalerTimeout = new CustomizationValue<TimeSpan>(
				() => Get().DefaultSignalerTimeout,
				v => Update(p => p with
				{
					DefaultSignalerTimeout = v,
				}));
			DefaultTimeComparisonTolerance = new CustomizationValue<TimeSpan>(
				() => Get().DefaultTimeComparisonTolerance,
				value =>
				{
					if (value < TimeSpan.Zero)
					{
						// ReSharper disable once LocalizableElement
						throw Tracing.WriteException(
							new ArgumentOutOfRangeException("tolerance", "The tolerance must not be negative."));
					}

					return Update(p => p with
					{
						DefaultTimeComparisonTolerance = value,
					});
				});
			TestCancellation = new CustomizationValue<TestCancellation?>(
				() => Get().TestCancellation,
				v => Update(p => p with
				{
					TestCancellation = v,
				}));
		}

		/// <inheritdoc cref="SettingsCustomizationValue.DefaultCheckInterval" />
		public ICustomizationValueSetter<TimeSpan> DefaultCheckInterval { get; }

		/// <inheritdoc cref="SettingsCustomizationValue.DefaultEventuallyTimeout" />
		public ICustomizationValueSetter<TimeSpan> DefaultEventuallyTimeout { get; }

		/// <inheritdoc cref="SettingsCustomizationValue.DefaultSignalerTimeout" />
		public ICustomizationValueSetter<TimeSpan> DefaultSignalerTimeout { get; }

		/// <inheritdoc cref="SettingsCustomizationValue.DefaultTimeComparisonTolerance" />
		public ICustomizationValueSetter<TimeSpan> DefaultTimeComparisonTolerance { get; }

		/// <inheritdoc cref="SettingsCustomizationValue.TestCancellation" />
		public ICustomizationValueSetter<TestCancellation?> TestCancellation { get; }

		/// <inheritdoc cref="ICustomizationValueUpdater{SettingsCustomizationValue}.Get()" />
		public SettingsCustomizationValue Get()
			=> _awexpectCustomization.Get(nameof(Settings), EmptyValue);

		/// <inheritdoc
		///     cref="ICustomizationValueUpdater{SettingsCustomizationValue}.Update(Func{SettingsCustomizationValue,SettingsCustomizationValue})" />
		public CustomizationLifetime Update(Func<SettingsCustomizationValue, SettingsCustomizationValue> update)
			=> _awexpectCustomization.Set(nameof(Settings), update(Get()));
	}

	/// <summary>
	///     Customize the settings.
	/// </summary>
	public record SettingsCustomizationValue
	{
		/// <summary>
		///     If set, applies the cancellation logic for all tests.
		/// </summary>
		public TestCancellation? TestCancellation { get; init; }

		/// <summary>
		///     The default interval for repeatedly checking the condition on an object.
		/// </summary>
		public TimeSpan DefaultCheckInterval { get; init; } = TimeSpan.FromMilliseconds(100);

		/// <summary>
		///     The default timeout until the expectations of <c>Eventually()</c> on a delegate must be met.
		/// </summary>
		public TimeSpan DefaultEventuallyTimeout { get; init; } = TimeSpan.FromSeconds(30);

		/// <summary>
		///     The default timeout for the <see cref="Signaler" />.
		/// </summary>
		public TimeSpan DefaultSignalerTimeout { get; init; } = TimeSpan.FromSeconds(30);

#if NET8_0_OR_GREATER
		/// <summary>
		///     The default tolerance for time comparisons.
		/// </summary>
		/// <remarks>
		///     In Windows the <see cref="DateTime" /> resolution is about 10 to 15
		///     milliseconds (<see href="https://stackoverflow.com/q/3140826/4003370" />), so
		///     comparing them as exact values might result in brittle tests.<br />
		///     Therefore, it is possible to specify a default tolerance that is used when a <see cref="DateTime" />,
		///     <see cref="DateTimeOffset" />, <see cref="DateOnly" />, <see cref="TimeOnly" /> or <see cref="TimeSpan" />
		///     subject is compared directly and no explicit tolerance is given. For <see cref="DateOnly" /> only the whole
		///     days of the tolerance count. It also applies to the items of a collection of <see cref="DateTime" />,
		///     <see cref="DateTimeOffset" /> or <see cref="TimeSpan" /> values compared with <c>IsEqualTo</c> or
		///     <c>All().AreEqualTo</c> without an explicit tolerance.<br />
		///     It is not used for property verifications, other collection expectations, members compared by equivalency
		///     or values compared as <see langword="object" />.
		/// </remarks>
#else
		/// <summary>
		///     The default tolerance for time comparisons.
		/// </summary>
		/// <remarks>
		///     In Windows the <see cref="DateTime" /> resolution is about 10 to 15
		///     milliseconds (<see href="https://stackoverflow.com/q/3140826/4003370" />), so
		///     comparing them as exact values might result in brittle tests.<br />
		///     Therefore, it is possible to specify a default tolerance that is used when a <see cref="DateTime" />,
		///     <see cref="DateTimeOffset" /> or <see cref="TimeSpan" /> subject is compared directly and no explicit
		///     tolerance is given. It also applies to the items of a collection of such values compared with
		///     <c>IsEqualTo</c> or <c>All().AreEqualTo</c> without an explicit tolerance.<br />
		///     It is not used for property verifications, other collection expectations, members compared by equivalency
		///     or values compared as <see langword="object" />.
		/// </remarks>
#endif
		public TimeSpan DefaultTimeComparisonTolerance { get; init; } = TimeSpan.Zero;
	}
}
