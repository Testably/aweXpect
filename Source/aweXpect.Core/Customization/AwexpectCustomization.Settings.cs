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

		/// <summary>
		///     The key of <see cref="TestCancellation" />, which every evaluation reads.
		/// </summary>
		internal const string TestCancellationKey = KeyPrefix + nameof(TestCancellation);

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
				TestCancellationKey, null);
		}

		/// <summary>
		///     The default interval for re-checking a condition, e.g. for <c>Eventually()</c> or for <c>Satisfies</c>
		///     with <c>Within</c>.
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
		///     days of the tolerance count. It also applies to the items of a collection of such values (or of their
		///     nullable counterparts) and to the values of a dictionary in every expectation, and its negation, that
		///     compares them with expected values, e.g. <c>IsEqualTo</c>, <c>Contains</c> or <c>ContainsValue</c>.<br />
		///     It is not used for property verifications, collection expectations that don't compare items with expected
		///     values, dictionary keys, members compared by equivalency or values compared as <see langword="object" />.
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
		///     tolerance is given. It also applies to the items of a collection of such values (or of their nullable
		///     counterparts) and to the values of a dictionary in every expectation, and its negation, that compares
		///     them with expected values, e.g. <c>IsEqualTo</c>, <c>Contains</c> or <c>ContainsValue</c>.<br />
		///     It is not used for property verifications, collection expectations that don't compare items with expected
		///     values, dictionary keys, members compared by equivalency or values compared as <see langword="object" />.
		/// </remarks>
#endif
		public ICustomizationValueSetter<TimeSpan> DefaultTimeComparisonTolerance { get; }

		/// <summary>
		///     If set, applies the cancellation logic for all tests.
		/// </summary>
		public ICustomizationValueSetter<TestCancellation?> TestCancellation { get; }
	}
}
