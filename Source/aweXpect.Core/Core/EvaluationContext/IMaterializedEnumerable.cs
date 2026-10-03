using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     A sequence that keeps the items it read from its source.
/// </summary>
/// <remarks>
///     Implemented by the materialized sequences of the <see cref="EvaluationContextExtensions" />, so that the items
///     read so far can be listed without reading further items from the source.
/// </remarks>
public interface IMaterializedEnumerable : IEnumerable, ICountable
{
	/// <summary>
	///     The items read from the source so far.
	/// </summary>
	IReadOnlyList<object?> MaterializedItems { get; }
}

/// <inheritdoc cref="IMaterializedEnumerable" />
public interface IMaterializedEnumerable<out T> : IEnumerable<T>, ICountable
{
	/// <inheritdoc cref="IMaterializedEnumerable.MaterializedItems" />
	IReadOnlyList<T> MaterializedItems { get; }
}
