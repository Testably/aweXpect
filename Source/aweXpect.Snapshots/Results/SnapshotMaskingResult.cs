using aweXpect.Core;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The result of a snapshot expectation after a literal masking, which can be switched to a regex pattern.
/// </summary>
public class SnapshotMaskingResult<TType, TThat> : SnapshotResult<TType, TThat>
{
	private readonly int _index;
	private readonly SnapshotOptions _options;

	internal SnapshotMaskingResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options,
		int index)
		: base(expectationBuilder, returnValue, options)
	{
		_options = options;
		_index = index;
	}

	/// <summary>
	///     Interprets the value of the masking as a regex pattern, whose matches are replaced with the substitutions of
	///     <see cref="System.Text.RegularExpressions.Regex.Replace(string, string)" />, e.g. <c>$1</c>.
	/// </summary>
	/// <remarks>
	///     An invalid pattern throws here. The pattern has a timeout of one second; for other options or another
	///     timeout, pass a <see cref="System.Text.RegularExpressions.Regex" /> to <c>.Masking(…)</c> instead.
	/// </remarks>
	public SnapshotResult<TType, TThat> AsRegex()
	{
		_options.Maskings[_index] = _options.Maskings[_index].AsRegex();
		return this;
	}
}
