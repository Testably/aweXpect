using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
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
	public class SettingsCustomization
	{
		private const string KeyPrefix = "aweXpect.Settings.";

		internal SettingsCustomization(IAwexpectCustomization awexpectCustomization)
		{
			DefaultCheckInterval = new CustomizationValue<TimeSpan>(awexpectCustomization,
				KeyPrefix + nameof(DefaultCheckInterval), TimeSpan.FromMilliseconds(100),
				interval =>
				{
					if (interval <= TimeSpan.Zero)
					{
						// ReSharper disable once LocalizableElement
						throw Tracing.WriteException(
							new ArgumentOutOfRangeException(nameof(interval), "The interval must be positive."));
					}
				});
			DefaultEventuallyTimeout = new CustomizationValue<TimeSpan>(awexpectCustomization,
				KeyPrefix + nameof(DefaultEventuallyTimeout), TimeSpan.FromSeconds(30),
				timeout => ThrowHelper.ThrowIfTimeoutIsNegative(timeout));
			DefaultSignalerTimeout = new CustomizationValue<TimeSpan>(awexpectCustomization,
				KeyPrefix + nameof(DefaultSignalerTimeout), TimeSpan.FromSeconds(30),
				timeout => ThrowHelper.ThrowIfTimeoutIsNegative(timeout));
			DefaultTimeComparisonTolerance = new CustomizationValue<TimeSpan>(awexpectCustomization,
				KeyPrefix + nameof(DefaultTimeComparisonTolerance), TimeSpan.Zero,
				tolerance =>
				{
					if (tolerance < TimeSpan.Zero)
					{
						// ReSharper disable once LocalizableElement
						throw Tracing.WriteException(
							new ArgumentOutOfRangeException(nameof(tolerance), "The tolerance must not be negative."));
					}
				});
			TestCancellation = new CustomizationValue<TestCancellation?>(awexpectCustomization,
				KeyPrefix + nameof(TestCancellation), null);
		}

		/// <summary>
		///     The default interval for repeatedly checking the condition on an object.
		/// </summary>
		public ICustomizationValueSetter<TimeSpan> DefaultCheckInterval { get; }

		/// <summary>
		///     The default timeout until the expectations of <c>Eventually()</c> on a delegate must be met.
		/// </summary>
		public ICustomizationValueSetter<TimeSpan> DefaultEventuallyTimeout { get; }

		/// <summary>
		///     The default timeout for the <see cref="Signaler" />.
		/// </summary>
		public ICustomizationValueSetter<TimeSpan> DefaultSignalerTimeout { get; }

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
		public ICustomizationValueSetter<TimeSpan> DefaultTimeComparisonTolerance { get; }

		/// <summary>
		///     If set, applies the cancellation logic for all tests.
		/// </summary>
		public ICustomizationValueSetter<TestCancellation?> TestCancellation { get; }
	}
}
