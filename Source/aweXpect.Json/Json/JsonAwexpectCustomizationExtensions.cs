#if NET8_0_OR_GREATER
using System;
using System.Text.Json;
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

	private sealed class CustomizationValue<TValue>(
		Func<TValue> getter,
		Func<TValue, CustomizationLifetime> setter)
		: ICustomizationValueSetter<TValue>
	{
		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
		public TValue Get() => getter();

		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
		public CustomizationLifetime Set(TValue value) => setter(value);
	}

	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	public class JsonCustomization : ICustomizationValueUpdater<JsonCustomizationValue>
	{
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal JsonCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			DefaultJsonDocumentOptions = new CustomizationValue<JsonDocumentOptions>(
				() => Get().DefaultJsonDocumentOptions,
				v => Update(p => p with
				{
					DefaultJsonDocumentOptions = v,
				}));
			DefaultJsonSerializerOptions = new CustomizationValue<JsonSerializerOptions>(
				() => Get().DefaultJsonSerializerOptions,
				v => Update(p => p with
				{
					DefaultJsonSerializerOptions = v,
				}));
		}

		/// <inheritdoc cref="JsonCustomizationValue.DefaultJsonDocumentOptions" />
		public ICustomizationValueSetter<JsonDocumentOptions> DefaultJsonDocumentOptions { get; }

		/// <inheritdoc cref="JsonCustomizationValue.DefaultJsonSerializerOptions" />
		public ICustomizationValueSetter<JsonSerializerOptions> DefaultJsonSerializerOptions { get; }

		/// <inheritdoc cref="ICustomizationValueUpdater{JsonCustomizationValue}.Get()" />
		public JsonCustomizationValue Get()
			=> _awexpectCustomization.Get(nameof(Json), new JsonCustomizationValue());

		/// <inheritdoc
		///     cref="ICustomizationValueUpdater{JsonCustomizationValue}.Update(Func{JsonCustomizationValue,JsonCustomizationValue})" />
		public CustomizationLifetime Update(Func<JsonCustomizationValue, JsonCustomizationValue> update)
			=> _awexpectCustomization.Set(nameof(Json), update(Get()));
	}

	/// <summary>
	///     Customize the JSON settings.
	/// </summary>
	public record JsonCustomizationValue
	{
		/// <summary>
		///     The default <see cref="JsonDocumentOptions" />.
		/// </summary>
		public JsonDocumentOptions DefaultJsonDocumentOptions { get; init; } = new()
		{
			AllowTrailingCommas = true,
		};

		/// <summary>
		///     The default <see cref="JsonSerializerOptions" />.
		/// </summary>
		public JsonSerializerOptions DefaultJsonSerializerOptions { get; init; } = new()
		{
			AllowTrailingCommas = true,
		};
	}
}
#endif
