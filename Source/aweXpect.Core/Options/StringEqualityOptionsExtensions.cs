using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that compare <see langword="string" />s with <see cref="StringEqualityOptions" />.
/// </summary>
/// <remarks>
///     A repeated option replaces the earlier one, and an option set to <see langword="false" /> is the same as not set.<br />
///     Only one match type, e.g. <c>AsWildcard()</c>, per expectation; repeating the same match type replaces it.<br />
///     A result that implements <see cref="ICopyOnOption{TOptions, TResult}" /> stays unchanged: the option is set on
///     a copy of its options, and the result returns a new one with them.
/// </remarks>
public static class StringEqualityOptionsExtensions
{
	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s,
	///     according to the <paramref name="ignoreCase" /> parameter.
	/// </summary>
	public static TResult IgnoringCase<TResult>(this TResult result, bool ignoreCase = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().IgnoringCase(ignoreCase));
		}

		result.Options.IgnoringCase(ignoreCase);
		return result;
	}

	/// <summary>
	///     Ignores the indentation when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreIndentation" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will remove the leading whitespace from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <c>IgnoringNewlineStyle()</c> redundant.<br />
	///     Any Unicode whitespace counts as indentation, e.g. also a non-breaking space.
	/// </remarks>
	public static TResult IgnoringIndentation<TResult>(this TResult result, bool ignoreIndentation = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().IgnoringIndentation(ignoreIndentation));
		}

		result.Options.IgnoringIndentation(ignoreIndentation);
		return result;
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreNewlineStyle" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace all occurrences of <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before
	///     comparing them.
	/// </remarks>
	public static TResult IgnoringNewlineStyle<TResult>(this TResult result, bool ignoreNewlineStyle = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().IgnoringNewlineStyle(ignoreNewlineStyle));
		}

		result.Options.IgnoringNewlineStyle(ignoreNewlineStyle);
		return result;
	}

	/// <summary>
	///     Ignores leading whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Note:<br />
	///     The position of the first mismatch in the failure message (its index, or its line and column) refers to the
	///     original subject, so it also counts the removed whitespace.
	/// </remarks>
	public static TResult IgnoringLeadingWhiteSpace<TResult>(this TResult result,
		bool ignoreLeadingWhiteSpace = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace));
		}

		result.Options.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
		return result;
	}

	/// <summary>
	///     Ignores trailing whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	public static TResult IgnoringTrailingWhiteSpace<TResult>(this TResult result,
		bool ignoreTrailingWhiteSpace = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace));
		}

		result.Options.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
		return result;
	}

	/// <summary>
	///     Normalizes the whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="normalizeWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace every run of whitespace, including line breaks, with a single space and
	///     remove the whitespace at the start and the end of the strings before comparing them.<br />
	///     Note:<br />
	///     The failure message shows the strings with normalized whitespace, and the position of the first mismatch
	///     refers to them.
	/// </remarks>
	public static TResult NormalizingWhiteSpace<TResult>(this TResult result, bool normalizeWhiteSpace = true)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().NormalizingWhiteSpace(normalizeWhiteSpace));
		}

		result.Options.NormalizingWhiteSpace(normalizeWhiteSpace);
		return result;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
	/// </summary>
	public static TResult Using<TResult>(this TResult result, IEqualityComparer<string> comparer)
		where TResult : IOptionsProvider<StringEqualityOptions>
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().Using(comparer));
		}

		result.Options.Using(comparer);
		return result;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
	/// </summary>
	public static TResult AsPrefix<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().AsPrefix());
		}

		result.Options.AsPrefix();
		return result;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
	/// </summary>
	public static TResult AsRegex<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringPatternMatchTypeOptions
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().AsRegex());
		}

		result.Options.AsRegex();
		return result;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern,
	///     applying the given <paramref name="regexOptions" />.
	/// </summary>
	public static TResult AsRegex<TResult>(this TResult result, RegexOptions regexOptions)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringPatternMatchTypeOptions
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().AsRegex(regexOptions));
		}

		result.Options.AsRegex(regexOptions);
		return result;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
	/// </summary>
	public static TResult AsSuffix<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().AsSuffix());
		}

		result.Options.AsSuffix();
		return result;
	}

	/// <summary>
	///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public static TResult AsWildcard<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringPatternMatchTypeOptions
	{
		if (result is ICopyOnOption<StringEqualityOptions, TResult> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Copy().AsWildcard());
		}

		result.Options.AsWildcard();
		return result;
	}
}
