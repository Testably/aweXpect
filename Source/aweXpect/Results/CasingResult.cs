using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying the casing of a string.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType, TThat}" />
/// </remarks>
public class CasingResult<TType, TThat> : AndOrResult<TType, TThat>
{
	private readonly CasingOptions _options;

	internal CasingResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		CasingOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <summary>
	///     Also rejects letters without an upper-case (lower-case) form and titlecase letters, instead of counting them
	///     as upper-cased (lower-cased).
	/// </summary>
	/// <remarks>
	///     For example, the lower-case letter <c>ß</c> has no single upper-case form, so <c>"STRAßE"</c> is then no
	///     longer upper-cased, and a titlecase letter like <c>ǅ</c> is then neither upper-cased nor lower-cased.
	/// </remarks>
	public AndOrResult<TType, TThat> IncludingUncasedLetters()
	{
		_options.IncludesUncasedLetters = true;
		return this;
	}
}
