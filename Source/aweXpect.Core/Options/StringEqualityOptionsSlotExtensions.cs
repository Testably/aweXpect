using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="StringEqualityOptions" /> on the <see cref="OptionsSlot{TOptions}" /> of an
///     expectation.
/// </summary>
public static class StringEqualityOptionsSlotExtensions
{
	/// <inheritdoc cref="StringEqualityOptions.IgnoringCase(bool)" />
	public static OptionsSlot<StringEqualityOptions> IgnoringCase(this OptionsSlot<StringEqualityOptions> slot,
		bool ignoreCase = true)
	{
		slot.Value = slot.Value.IgnoringCase(ignoreCase);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringIndentation(bool)" />
	public static OptionsSlot<StringEqualityOptions> IgnoringIndentation(this OptionsSlot<StringEqualityOptions> slot,
		bool ignoreIndentation = true)
	{
		slot.Value = slot.Value.IgnoringIndentation(ignoreIndentation);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringNewlineStyle(bool)" />
	public static OptionsSlot<StringEqualityOptions> IgnoringNewlineStyle(this OptionsSlot<StringEqualityOptions> slot,
		bool ignoreNewlineStyle = true)
	{
		slot.Value = slot.Value.IgnoringNewlineStyle(ignoreNewlineStyle);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringLeadingWhiteSpace(bool)" />
	public static OptionsSlot<StringEqualityOptions> IgnoringLeadingWhiteSpace(
		this OptionsSlot<StringEqualityOptions> slot, bool ignoreLeadingWhiteSpace = true)
	{
		slot.Value = slot.Value.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.IgnoringTrailingWhiteSpace(bool)" />
	public static OptionsSlot<StringEqualityOptions> IgnoringTrailingWhiteSpace(
		this OptionsSlot<StringEqualityOptions> slot, bool ignoreTrailingWhiteSpace = true)
	{
		slot.Value = slot.Value.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.NormalizingWhiteSpace(bool)" />
	public static OptionsSlot<StringEqualityOptions> NormalizingWhiteSpace(
		this OptionsSlot<StringEqualityOptions> slot, bool normalizeWhiteSpace = true)
	{
		slot.Value = slot.Value.NormalizingWhiteSpace(normalizeWhiteSpace);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.Using(IEqualityComparer{string})" />
	public static OptionsSlot<StringEqualityOptions> Using(this OptionsSlot<StringEqualityOptions> slot,
		IEqualityComparer<string> comparer)
	{
		slot.Value = slot.Value.Using(comparer);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsBlock()" />
	public static OptionsSlot<StringEqualityOptions> AsBlock(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.AsBlock();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsPrefix()" />
	public static OptionsSlot<StringEqualityOptions> AsPrefix(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.AsPrefix();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsRegex()" />
	public static OptionsSlot<StringEqualityOptions> AsRegex(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.AsRegex();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsRegex(RegexOptions)" />
	public static OptionsSlot<StringEqualityOptions> AsRegex(this OptionsSlot<StringEqualityOptions> slot,
		RegexOptions regexOptions)
	{
		slot.Value = slot.Value.AsRegex(regexOptions);
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsSuffix()" />
	public static OptionsSlot<StringEqualityOptions> AsSuffix(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.AsSuffix();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.AsWildcard()" />
	public static OptionsSlot<StringEqualityOptions> AsWildcard(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.AsWildcard();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.Containing()" />
	public static OptionsSlot<StringEqualityOptions> Containing(this OptionsSlot<StringEqualityOptions> slot)
	{
		slot.Value = slot.Value.Containing();
		return slot;
	}

	/// <inheritdoc cref="StringEqualityOptions.SetMatchType(StringMatchType, string)" />
	public static OptionsSlot<StringEqualityOptions> SetMatchType(this OptionsSlot<StringEqualityOptions> slot,
		StringMatchType matchType, string optionName)
	{
		slot.Value = slot.Value.SetMatchType(matchType, optionName);
		return slot;
	}
}
