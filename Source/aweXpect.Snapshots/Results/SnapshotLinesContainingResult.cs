using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The result of a snapshot expectation after <c>.Ignoring.LinesContaining(…)</c>, which additionally allows
///     setting the options of that line rule.
/// </summary>
/// <remarks>
///     The options of aweXpect's <c>Contains</c>, e.g. <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or <c>.AsRegex()</c>,
///     apply to the line rule only, not to the comparison with the snapshot. They have to follow the line rule directly,
///     as any other option returns a <see cref="SnapshotResult{TType, TThat}" />.
/// </remarks>
public sealed class SnapshotLinesContainingResult<TType, TThat> : SnapshotResult<TType, TThat>,
	IOptionsProvider<StringEqualityOptions>,
	IStringPatternMatchTypeOptions
{
	private readonly SnapshotIgnoringLinesContaining _ignoring;

	internal SnapshotLinesContainingResult(ExpectationBuilder expectationBuilder, TThat returnValue,
		SnapshotOptions options, SnapshotIgnoringLinesContaining ignoring)
		: base(expectationBuilder, returnValue, options)
	{
		_ignoring = ignoring;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _ignoring.Options;
}
