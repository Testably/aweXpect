using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying the equality of a <see langword="char" />.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType, TThat}" />
/// </remarks>
public class CharEqualityResult<TType, TThat> : AndOrResult<TType, TThat>
{
	private readonly CharEqualityOptions _options;

	internal CharEqualityResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		CharEqualityOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <summary>
	///     Ignores casing when comparing the <see langword="char" />s,
	///     according to the <paramref name="ignoreCase" /> parameter.
	/// </summary>
	/// <remarks>
	///     The comparison is the same as for <see langword="string" />s, i.e.
	///     <see cref="System.StringComparison.OrdinalIgnoreCase" />.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">The casing is already specified.</exception>
	public AndOrResult<TType, TThat> IgnoringCase(bool ignoreCase = true)
	{
		_options.IgnoringCase(ignoreCase);
		return this;
	}
}
