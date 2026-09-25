using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class StringEqualityResultExtensions
{
	/// <remarks>
	///     The result offers no <c>AsRegex()</c>, so the match type is switched through the options, as an extension
	///     would do.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> AsRegexThroughOptions<TType, TThat>(
		this StringEqualityResult<TType, TThat> result)
	{
		(result as IOptionsProvider<StringEqualityOptions>).Options.AsRegex();
		return result;
	}
}
