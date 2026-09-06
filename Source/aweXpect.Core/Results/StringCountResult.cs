using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string collection has a specified number of items.
/// </summary>
/// <remarks>
///     <seealso cref="CountResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringCountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: StringCountResult<TType, TThat,
		StringCountResult<TType, TThat>>(
		expectationBuilder,
		returnValue,
		quantifier,
		options);

/// <summary>
///     The result for verifying that a string collection has a specified number of items.
/// </summary>
/// <remarks>
///     <seealso cref="CountResult{TType,TThat,TSelf}" />
/// </remarks>
public class StringCountResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	StringEqualityOptions options)
	: CountResult<TType, TThat, TSelf>(expectationBuilder, returnValue, quantifier),
		IOptionsProvider<StringEqualityOptions>
	where TSelf : StringCountResult<TType, TThat, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
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
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.<br />
	///     Any Unicode white-space counts as indentation, e.g. also a non-breaking space.
	/// </remarks>
	public TSelf IgnoringIndentation(bool ignoreIndentation = true)
	{
		options.IgnoringIndentation(ignoreIndentation);
		return (TSelf)this;
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s.
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
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public TSelf Using(
		IEqualityComparer<string> comparer)
	{
		options.UsingComparer(comparer);
		return (TSelf)this;
	}
}
