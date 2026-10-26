using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="StringEqualityOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class StringEqualityOptionsHolderExtensions
{
	/// <inheritdoc cref="StringEqualityOptions.IgnoringCase(bool)" />
	public static OptionsHolder<StringEqualityOptions> IgnoringCase(this OptionsHolder<StringEqualityOptions> holder,
		bool ignoreCase = true)
	{
		holder.Value = holder.Value.IgnoringCase(ignoreCase);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringIndentation(bool)" />
	public static OptionsHolder<StringEqualityOptions> IgnoringIndentation(this OptionsHolder<StringEqualityOptions> holder,
		bool ignoreIndentation = true)
	{
		holder.Value = holder.Value.IgnoringIndentation(ignoreIndentation);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringNewlineStyle(bool)" />
	public static OptionsHolder<StringEqualityOptions> IgnoringNewlineStyle(this OptionsHolder<StringEqualityOptions> holder,
		bool ignoreNewlineStyle = true)
	{
		holder.Value = holder.Value.IgnoringNewlineStyle(ignoreNewlineStyle);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringLeadingWhiteSpace(bool)" />
	public static OptionsHolder<StringEqualityOptions> IgnoringLeadingWhiteSpace(
		this OptionsHolder<StringEqualityOptions> holder, bool ignoreLeadingWhiteSpace = true)
	{
		holder.Value = holder.Value.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringTrailingWhiteSpace(bool)" />
	public static OptionsHolder<StringEqualityOptions> IgnoringTrailingWhiteSpace(
		this OptionsHolder<StringEqualityOptions> holder, bool ignoreTrailingWhiteSpace = true)
	{
		holder.Value = holder.Value.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.NormalizingWhiteSpace(bool)" />
	public static OptionsHolder<StringEqualityOptions> NormalizingWhiteSpace(
		this OptionsHolder<StringEqualityOptions> holder, bool normalizeWhiteSpace = true)
	{
		holder.Value = holder.Value.NormalizingWhiteSpace(normalizeWhiteSpace);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.Using(IEqualityComparer{string})" />
	public static OptionsHolder<StringEqualityOptions> Using(this OptionsHolder<StringEqualityOptions> holder,
		IEqualityComparer<string> comparer)
	{
		holder.Value = holder.Value.Using(comparer);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsBlock()" />
	public static OptionsHolder<StringEqualityOptions> AsBlock(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.AsBlock();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsPrefix()" />
	public static OptionsHolder<StringEqualityOptions> AsPrefix(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.AsPrefix();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsRegex()" />
	public static OptionsHolder<StringEqualityOptions> AsRegex(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.AsRegex();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsRegex(RegexOptions)" />
	public static OptionsHolder<StringEqualityOptions> AsRegex(this OptionsHolder<StringEqualityOptions> holder,
		RegexOptions regexOptions)
	{
		holder.Value = holder.Value.AsRegex(regexOptions);
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsSuffix()" />
	public static OptionsHolder<StringEqualityOptions> AsSuffix(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.AsSuffix();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsWildcard()" />
	public static OptionsHolder<StringEqualityOptions> AsWildcard(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.AsWildcard();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.Containing()" />
	public static OptionsHolder<StringEqualityOptions> Containing(this OptionsHolder<StringEqualityOptions> holder)
	{
		holder.Value = holder.Value.Containing();
		return holder;
	}

	/// <inheritdoc cref="StringEqualityOptions.SetMatchType(StringMatchType, string)" />
	public static OptionsHolder<StringEqualityOptions> SetMatchType(this OptionsHolder<StringEqualityOptions> holder,
		StringMatchType matchType, string optionName)
	{
		holder.Value = holder.Value.SetMatchType(matchType, optionName);
		return holder;
	}
}
