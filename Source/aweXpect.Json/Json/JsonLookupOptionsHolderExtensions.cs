using aweXpect.Core;
using aweXpect.Json.Options;

namespace aweXpect.Json;

/// <summary>
///     The option methods of <see cref="JsonLookupOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class JsonLookupOptionsHolderExtensions
{
	/// <inheritdoc cref="JsonLookupOptions.TreatingNullAsAbsent(bool)" />
	public static OptionsHolder<JsonLookupOptions> TreatingNullAsAbsent(this OptionsHolder<JsonLookupOptions> holder,
		bool treatNullAsAbsent = true)
	{
		holder.Value = holder.Value.TreatingNullAsAbsent(treatNullAsAbsent);
		return holder;
	}
}
