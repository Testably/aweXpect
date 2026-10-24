using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Json;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for JSON.
/// </summary>
public static class JsonCustomizationExtensions
{
	private static JsonCustomization? _lastCustomization;

	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	/// <remarks>
	///     The settings are read from the <paramref name="awexpectCustomization" /> on every access, so the group of the
	///     last customization is reused instead of being created anew in every expectation.
	/// </remarks>
	public static JsonCustomization Json(this AwexpectCustomization awexpectCustomization)
	{
		JsonCustomization? cached = _lastCustomization;
		if (cached is not null && ReferenceEquals(cached.Owner, awexpectCustomization))
		{
			return cached;
		}

		cached = new JsonCustomization(awexpectCustomization);
		_lastCustomization = cached;
		return cached;
	}

	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	/// <remarks>
	///     Each value is stored on its own, so that it can be set and restored independently of the other values.
	/// </remarks>
	public class JsonCustomization
	{
		/// <remarks>
		///     A single instance, so that <see cref="GetCustomizedSerializerOptions" /> can tell it from a set value.
		/// </remarks>
		private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
		{
			AllowTrailingCommas = true,
		};

		internal JsonCustomization(IAwexpectCustomization awexpectCustomization)
		{
			Owner = awexpectCustomization;
			DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(awexpectCustomization,
				"aweXpect.Json.DefaultJsonDocumentOptions", new JsonDocumentOptions
				{
					AllowTrailingCommas = true,
				});
			DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(awexpectCustomization,
				"aweXpect.Json.DefaultJsonSerializerOptions", DefaultSerializerOptions,
				value =>
				{
					if (value is null)
					{
						throw Tracing.WriteException(
							new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
					}
				});
		}

		internal IAwexpectCustomization Owner { get; }

		/// <summary>
		///     The default <see cref="JsonDocumentOptions" />.
		/// </summary>
		public ICustomizationValueSetter<JsonDocumentOptions> DefaultJsonDocumentOptions { get; }

		/// <summary>
		///     The default <see cref="JsonSerializerOptions" />.
		/// </summary>
		public ICustomizationValueSetter<JsonSerializerOptions> DefaultJsonSerializerOptions { get; }

		/// <summary>
		///     The <see cref="DefaultJsonSerializerOptions" />, or <see langword="null" /> while no value is set.
		/// </summary>
		internal JsonSerializerOptions? GetCustomizedSerializerOptions()
		{
			JsonSerializerOptions options = DefaultJsonSerializerOptions.Get();
			return ReferenceEquals(options, DefaultSerializerOptions) ? null : options;
		}
	}
}
