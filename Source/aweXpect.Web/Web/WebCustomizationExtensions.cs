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
			ContentProcessors = new CopyingSetter<IContentProcessor>(
				new CustomizationValue<IReadOnlyList<IContentProcessor>>(awexpectCustomization,
					"aweXpect.Web.ContentProcessors",
					Array.AsReadOnly<IContentProcessor>([
						new JsonContentProcessor(),
						new StringContentProcessor(),
						new BinaryContentProcessor(),
					]),
					value =>
					{
						if (value is null)
						{
							throw Tracing.WriteException(
								new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
						}
					}));
		}

		/// <summary>
		///     The content processors to use to format the <see cref="HttpContent" />.
		/// </summary>
		/// <remarks>
		///     The list is copied, so changing it afterwards does not change the setting.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<IContentProcessor>> ContentProcessors { get; }

		/// <summary>
		///     Stores a read-only copy of the list, as a setting would otherwise change with a mutable list it was set
		///     from or that was cast from the returned list, which would bypass the scoping of the setting.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> list is passed on unchanged, so that the validation of the
		///     <paramref name="inner" /> value rejects it.
		/// </remarks>
		private sealed class CopyingSetter<TItem>(ICustomizationValueSetter<IReadOnlyList<TItem>> inner)
			: ICustomizationValueSetter<IReadOnlyList<TItem>>
		{
			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
			public IReadOnlyList<TItem> Get() => inner.Get();

			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
			public CustomizationLifetime Set(IReadOnlyList<TItem> value)
				=> inner.Set(value is null ? value! : Array.AsReadOnly(value.ToArray()));
		}
	}
}
