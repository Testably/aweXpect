using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a specific item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class StringHasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: StringHasItemResult<TCollection,
		StringHasItemResult<TCollection>>(
		expectationBuilder,
		collection,
		collectionIndexOptions,
		options);

/// <summary>
///     The result for verifying that a collection has a specific item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class StringHasItemResult<TCollection, TSelf>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
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
	///     Enabling this option will remove the leading white-space from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.
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
	///     Ignores leading white-space when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Note:<br />
	///     This affects the index of first mismatch, as the removed whitespace is also ignored for the index calculation!
	/// </remarks>
	public TSelf IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		options.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores trailing white-space when comparing <see langword="string" />s,
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
		options.UsingComparer(comparer);
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
