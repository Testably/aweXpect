using System;
using System.Net.Http;
using aweXpect.Customization;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Web;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for aweXpect.Web.
/// </summary>
public static class WebAwexpectCustomizationExtensions
{
	/// <summary>
	///     Customize the aweXpect.Web settings.
	/// </summary>
	public static WebCustomization Web(this AwexpectCustomization awexpectCustomization)
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
	///     Customize the Web settings.
	/// </summary>
	public class WebCustomization : ICustomizationValueUpdater<WebCustomizationValue>
	{
		private readonly IAwexpectCustomization _awexpectCustomization;

		internal WebCustomization(IAwexpectCustomization awexpectCustomization)
		{
			_awexpectCustomization = awexpectCustomization;
			ContentProcessors = new CustomizationValue<IContentProcessor[]>(
				() => Get().ContentProcessors,
				v => Update(p => p with
				{
					ContentProcessors = v,
				}));
		}

		/// <inheritdoc cref="WebCustomizationValue.ContentProcessors" />
		public ICustomizationValueSetter<IContentProcessor[]> ContentProcessors { get; }

		/// <inheritdoc cref="ICustomizationValueUpdater{WebCustomizationValue}.Get()" />
		public WebCustomizationValue Get()
			=> _awexpectCustomization.Get(nameof(Web), new WebCustomizationValue());

		/// <inheritdoc
		///     cref="ICustomizationValueUpdater{WebCustomizationValue}.Update(Func{WebCustomizationValue,WebCustomizationValue})" />
		public CustomizationLifetime Update(Func<WebCustomizationValue, WebCustomizationValue> update)
			=> _awexpectCustomization.Set(nameof(Web), update(Get()));
	}

	/// <summary>
	///     Customize the aweXpect.Web settings.
	/// </summary>
	public record WebCustomizationValue
	{
		/// <summary>
		///     The content processors to use to format the <see cref="HttpContent" />.
		/// </summary>
		public IContentProcessor[] ContentProcessors { get; init; } =
		[
			new JsonContentProcessor(),
			new StringContentProcessor(),
			new BinaryContentProcessor(),
		];
	}
}
