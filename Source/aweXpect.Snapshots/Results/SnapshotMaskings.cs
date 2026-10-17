using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The maskings that can be added to a snapshot expectation, with the same names and semantics as the factories of
///     <see cref="SnapshotMasking" /> for all snapshots.
/// </summary>
/// <remarks>
///     The maskings of the expectation are applied in the order in which they are added, then the maskings for all
///     snapshots, so a specific one goes before a general one. The snapshot and the failure contain the masked subject.
/// </remarks>
public sealed class SnapshotMaskings<TType, TThat>
{
	private readonly SnapshotOptions _options;
	private readonly SnapshotResult<TType, TThat> _result;

	internal SnapshotMaskings(SnapshotResult<TType, TThat> result, SnapshotOptions options)
	{
		_result = result;
		_options = options;
	}

	/// <inheritdoc cref="SnapshotMasking.Dates()" />
	public SnapshotResult<TType, TThat> Dates()
		=> Add(SnapshotMasking.Dates());

	/// <inheritdoc cref="SnapshotMasking.Guids()" />
	public SnapshotResult<TType, TThat> Guids()
		=> Add(SnapshotMasking.Guids());

	/// <inheritdoc cref="SnapshotMasking.Member(string)" />
	public SnapshotResult<TType, TThat> Member(string memberName)
		=> AddStructural(SnapshotMasking.Member(memberName));

	/// <inheritdoc cref="SnapshotMasking.Member(string, string)" />
	public SnapshotResult<TType, TThat> Member(string memberName, string replacement)
		=> AddStructural(SnapshotMasking.Member(memberName, replacement));

	/// <inheritdoc cref="SnapshotMasking.Regex(string, string)" />
	public SnapshotResult<TType, TThat> Regex(string pattern, string replacement)
		=> Add(SnapshotMasking.Regex(pattern, replacement));

	/// <inheritdoc cref="SnapshotMasking.Regex(System.Text.RegularExpressions.Regex, string)" />
	public SnapshotResult<TType, TThat> Regex(System.Text.RegularExpressions.Regex pattern, string replacement)
		=> Add(SnapshotMasking.Regex(pattern, replacement));

	/// <inheritdoc cref="SnapshotMasking.Text(string, string)" />
	public SnapshotResult<TType, TThat> Text(string value, string replacement)
		=> Add(SnapshotMasking.Text(value, replacement));

	/// <inheritdoc cref="SnapshotMasking.Type{T}()" />
	public SnapshotResult<TType, TThat> Type<T>()
		=> AddStructural(SnapshotMasking.Type<T>());

	/// <inheritdoc cref="SnapshotMasking.Type{T}(string)" />
	public SnapshotResult<TType, TThat> Type<T>(string replacement)
		=> AddStructural(SnapshotMasking.Type<T>(replacement));

	private SnapshotResult<TType, TThat> Add(SnapshotMasking masking)
	{
		_options.Maskings.Add(masking);
		return _result;
	}

	private SnapshotResult<TType, TThat> AddStructural(SnapshotMasking masking)
	{
		StructuralRules.ThrowIfString<TType>();
		return Add(masking);
	}
}
