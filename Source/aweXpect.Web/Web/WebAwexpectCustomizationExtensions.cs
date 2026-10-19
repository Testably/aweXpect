using System;
using System.Net.Http;
using aweXpect.Core;
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

	/// <summary>
	///     Customize the Web settings.
	/// </summary>
	public class WebCustomization
	{
		internal WebCustomization(IAwexpectCustomization awexpectCustomization)
		{
			ContentProcessors = new CustomizationValue<IContentProcessor[]>(awexpectCustomization,
				"aweXpect.Web.ContentProcessors",
				[
					new JsonContentProcessor(),
					new StringContentProcessor(),
					new BinaryContentProcessor(),
				],
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
		///     The content processors to use to format the <see cref="HttpContent" />.
		/// </summary>
		public ICustomizationValueSetter<IContentProcessor[]> ContentProcessors { get; }
	}
}
