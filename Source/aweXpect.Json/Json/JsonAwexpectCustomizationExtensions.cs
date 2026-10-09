using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Json;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for JSON.
/// </summary>
public static class JsonAwexpectCustomizationExtensions
{
	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	public static JsonCustomization Json(this AwexpectCustomization awexpectCustomization)
		=> new(awexpectCustomization);

	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	/// <remarks>
	///     Each value is stored on its own, so that it can be set and restored independently of the other values.
	/// </remarks>
	public class JsonCustomization
	{
		internal JsonCustomization(IAwexpectCustomization awexpectCustomization)
		{
			DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(awexpectCustomization,
				"aweXpect.Json.DefaultJsonDocumentOptions", new JsonDocumentOptions
				{
					AllowTrailingCommas = true,
				});
			DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(awexpectCustomization,
				"aweXpect.Json.DefaultJsonSerializerOptions", new JsonSerializerOptions
				{
					AllowTrailingCommas = true,
				},
				value =>
				{
					if (value is null)
					{
						throw Tracing.WriteException(
							new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
					}
				});
		}

		/// <summary>
		///     The default <see cref="JsonDocumentOptions" />.
		/// </summary>
		public ICustomizationValueSetter<JsonDocumentOptions> DefaultJsonDocumentOptions { get; }

		/// <summary>
		///     The default <see cref="JsonSerializerOptions" />.
		/// </summary>
		public ICustomizationValueSetter<JsonSerializerOptions> DefaultJsonSerializerOptions { get; }
	}
}
