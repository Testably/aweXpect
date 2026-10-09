using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Customization;

public partial class AwexpectCustomization
{
	private FormattingCustomization? _formatting;

	/// <summary>
	///     Customize the formatting settings.
	/// </summary>
	public FormattingCustomization Formatting()
	{
		_formatting ??= new FormattingCustomization(this);
		return _formatting;
	}

	/// <summary>
	///     Customize the formatting settings.
	/// </summary>
	public class FormattingCustomization : IAwexpectCustomization
	{
		private const string KeyPrefix = "aweXpect.Formatting.";
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal FormattingCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			MaximumNumberOfCollectionItems = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MaximumNumberOfCollectionItems), 10,
				count =>
				{
					if (count <= 0)
					{
						// ReSharper disable once LocalizableElement
						throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(count),
							"The maximum number of collection items must be positive."));
					}
				});
			MaximumStringLength = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MaximumStringLength), 100,
				length => ThrowHelper.ThrowIfCountIsNegative(length, "maximum string length"));
			MinimumNumberOfCharactersAfterStringDifference = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MinimumNumberOfCharactersAfterStringDifference), 45,
				count => ThrowHelper.ThrowIfCountIsNegative(count,
					"minimum number of characters after the string difference"));
		}

		/// <summary>
		///     The maximum number of displayed items in a collection.
		/// </summary>
		public ICustomizationValueSetter<int> MaximumNumberOfCollectionItems { get; }

		/// <summary>
		///     The maximum length of a <see langword="string" /> before it gets truncated.
		/// </summary>
		public ICustomizationValueSetter<int> MaximumStringLength { get; }

		/// <summary>
		///     The minimum number of characters included after the first mismatch in the string difference.
		/// </summary>
		public ICustomizationValueSetter<int> MinimumNumberOfCharactersAfterStringDifference { get; }

		/// <inheritdoc cref="IAwexpectCustomization.Get{TValue}(string, TValue)" />
		TValue IAwexpectCustomization.Get<TValue>(string key, TValue defaultValue)
			=> _awexpectCustomization.Get(key, defaultValue);

		/// <inheritdoc cref="IAwexpectCustomization.Set{TValue}(string, TValue)" />
		CustomizationLifetime IAwexpectCustomization.Set<TValue>(string key, TValue value)
			=> _awexpectCustomization.Set(key, value);
	}
}
