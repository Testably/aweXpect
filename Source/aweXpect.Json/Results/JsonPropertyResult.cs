using aweXpect.Core;
using aweXpect.Json.Options;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a JSON value has or does not have a property or a value at a path.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class JsonPropertyResult<TType, TThat> : AndOrResult<TType, TThat>, IOptionsProvider<JsonLookupOptions>
{
	private readonly OptionsHolder<JsonLookupOptions> _options;

	internal JsonPropertyResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsHolder<JsonLookupOptions> options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<JsonLookupOptions> IOptionsProvider<JsonLookupOptions>.Options => _options;

	/// <inheritdoc cref="JsonLookupOptions.TreatingNullAsAbsent(bool)" />
	public JsonPropertyResult<TType, TThat> TreatingNullAsAbsent(bool treatNullAsAbsent = true)
	{
		_options.TreatingNullAsAbsent(treatNullAsAbsent);
		return this;
	}
}
