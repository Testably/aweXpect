using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionIndexOptionsExtensions" /> and
///     <see cref="StringEqualityOptionsExtensions" />.
/// </remarks>
public class StringHasItemResult<TCollection, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
	IOptionsHolderSource<StringEqualityOptions> options)
	: AndOrResult<TCollection, TThat, StringHasItemResult<TCollection, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionIndexOptions>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly TThat _returnValue = returnValue;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => options.Holder;

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a block of lines, which may be indented as a whole.
	/// </summary>
	/// <remarks>
	///     The block must consist of the same lines as the item. All its lines must share the same whitespace prefix in
	///     the item, so the relative indentation within the block is still compared.<br />
	///     A line that consists only of whitespace matches any line that consists only of whitespace.<br />
	///     The newline style is always ignored, and a single trailing line terminator does not start a new line,
	///     so <c>"a\nb\n"</c> has the same two lines as <c>"a\nb"</c>.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">
	///     A match type or an option that changes the lines, e.g. the indentation, is already specified.
	/// </exception>
	public StringBlockHasItemResult<TCollection, TThat> AsBlock()
	{
		options.Holder.AsBlock();
		return new StringBlockHasItemResult<TCollection, TThat>(ExpectationBuilder, _returnValue, collectionIndexOptions,
			options);
	}
}
