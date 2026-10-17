using System;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The ignorings that can be added to a snapshot expectation, with the same names and semantics as the factories of
///     <see cref="SnapshotIgnoring" /> for all snapshots.
/// </summary>
/// <remarks>
///     The ignorings of the expectation are applied together with the ignorings for all snapshots, and win over every
///     masking. Member and type rules only apply to snapshots of objects, as a <see cref="string" /> is compared as raw
///     text, and line rules only to snapshots of a <see cref="string" />.
/// </remarks>
public sealed class SnapshotIgnorings<TType, TThat>
{
	private readonly SnapshotOptions _options;
	private readonly SnapshotResult<TType, TThat> _result;

	internal SnapshotIgnorings(SnapshotResult<TType, TThat> result, SnapshotOptions options)
	{
		_result = result;
		_options = options;
	}

	/// <inheritdoc cref="SnapshotIgnoring.Lines(Func{string, bool})" />
	public SnapshotResult<TType, TThat> Lines(Func<string, bool> predicate)
		=> AddLineRule(SnapshotIgnoring.Lines(predicate));

	/// <inheritdoc cref="SnapshotIgnoring.LinesContaining(string)" />
	public SnapshotLinesContainingResult<TType, TThat> LinesContaining(string text)
	{
		SnapshotIgnoringLinesContaining ignoring = SnapshotIgnoring.LinesContaining(text);
		AddLineRule(ignoring);
		return _result.WithLineRule(ignoring);
	}

	/// <inheritdoc cref="SnapshotIgnoring.Member(string)" />
	public SnapshotResult<TType, TThat> Member(string memberName)
		=> Add(SnapshotIgnoring.Member(memberName));

	/// <inheritdoc cref="SnapshotIgnoring.Member{T}()" />
	public SnapshotResult<TType, TThat> Member<T>()
		=> Add(SnapshotIgnoring.Member<T>());

	private SnapshotResult<TType, TThat> Add(SnapshotIgnoring ignoring)
	{
		StructuralRules.ThrowIfString<TType>();
		_options.Ignorings.Add(ignoring);
		return _result;
	}

	private SnapshotResult<TType, TThat> AddLineRule(SnapshotIgnoring ignoring)
	{
		StructuralRules.ThrowIfNotString<TType>();
		_options.Ignorings.Add(ignoring);
		return _result;
	}
}
