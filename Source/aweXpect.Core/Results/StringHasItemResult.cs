using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class StringHasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: StringHasItemResult<TCollection,
		StringHasItemResult<TCollection>>(
		expectationBuilder,
		collection,
		collectionIndexOptions,
		options)
{
	private readonly IThat<TCollection?> _collection = collection;
	private readonly CollectionIndexOptions _collectionIndexOptions = collectionIndexOptions;
	private readonly StringEqualityOptions _options = options;

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
	public StringBlockHasItemResult<TCollection> AsBlock()
	{
		_options.AsBlock();
		return new StringBlockHasItemResult<TCollection>(ExpectationBuilder, _collection, _collectionIndexOptions,
			_options);
	}
}

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class StringHasItemResult<TCollection, TSelf>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: HasItemResult<TCollection>(expectationBuilder, collection, collectionIndexOptions),
		IOptionsProvider<StringEqualityOptions>
	where TSelf : StringHasItemResult<TCollection, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s,
	///     according to the <paramref name="ignoreCase" /> parameter.
	/// </summary>
	public TSelf IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores the indentation when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreIndentation" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will remove the leading whitespace from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.<br />
	///     Any Unicode whitespace counts as indentation, e.g. also a non-breaking space.
	/// </remarks>
	public TSelf IgnoringIndentation(bool ignoreIndentation = true)
	{
		options.IgnoringIndentation(ignoreIndentation);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreNewlineStyle" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace all occurrences of <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before
	///     comparing them.
	/// </remarks>
	public TSelf IgnoringNewlineStyle(bool ignoreNewlineStyle = true)
	{
		options.IgnoringNewlineStyle(ignoreNewlineStyle);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores leading whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	public TSelf IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		options.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores trailing whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	public TSelf IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
	{
		options.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
		return (TSelf)this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public TSelf Using(
		IEqualityComparer<string> comparer)
	{
		options.Using(comparer);
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
	/// </summary>
	public TSelf AsPrefix()
	{
		options.AsPrefix();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	public TSelf AsRegex()
	{
		options.AsRegex();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern,
	///     applying the given <paramref name="regexOptions" />.
	/// </summary>
	public TSelf AsRegex(RegexOptions regexOptions)
	{
		options.AsRegex(regexOptions);
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
	/// </summary>
	public TSelf AsSuffix()
	{
		options.AsSuffix();
		return (TSelf)this;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public TSelf AsWildcard()
	{
		options.AsWildcard();
		return (TSelf)this;
	}
}
