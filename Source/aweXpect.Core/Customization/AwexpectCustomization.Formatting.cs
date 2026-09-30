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
	public class FormattingCustomization
	{
		private const string KeyPrefix = "aweXpect.Formatting.";

		internal FormattingCustomization(IAwexpectCustomization awexpectCustomization)
		{
			MaximumNumberOfCollectionItems = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MaximumNumberOfCollectionItems), 10);
			MaximumStringLength = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MaximumStringLength), 100);
			MinimumNumberOfCharactersAfterStringDifference = new CustomizationValue<int>(awexpectCustomization,
				KeyPrefix + nameof(MinimumNumberOfCharactersAfterStringDifference), 45);
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
	}
}
