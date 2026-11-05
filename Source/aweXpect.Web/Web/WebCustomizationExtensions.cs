using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Web;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for aweXpect.Web.
/// </summary>
public static class WebCustomizationExtensions
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
			ContentProcessors = new CustomizationListValue<IContentProcessor>(awexpectCustomization,
				"aweXpect.Web.ContentProcessors",
				Array.AsReadOnly<IContentProcessor>([
					new JsonContentProcessor(),
					new StringContentProcessor(),
					new BinaryContentProcessor(),
				]), ThrowIfContainsNull);
		}

		/// <summary>
		///     The content processors to use to format the <see cref="HttpContent" />.
		/// </summary>
		/// <remarks>
		///     The list is copied, so changing it afterwards does not change the setting.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<IContentProcessor>> ContentProcessors { get; }

		private static void ThrowIfContainsNull<TItem>(IReadOnlyList<TItem> value)
		{
			if (value.Any(item => item is null))
			{
				throw Tracing.WriteException(
					new ArgumentException("The 'value' cannot contain null.", nameof(value)));
			}
		}
	}
}
