#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     An asynchronous sequence that keeps the items it received from its source.
/// </summary>
/// <remarks>
///     Implemented by the materialized asynchronous sequences of the <see cref="EvaluationContextExtensions" />.
/// </remarks>
public interface IMaterializedAsyncEnumerable<T> : IAsyncEnumerable<T>, ICountable
{
	/// <summary>
	///     The items received from the source so far.
	/// </summary>
	IReadOnlyList<T> MaterializedItems { get; }

	/// <summary>
	///     Receives items from the source until more than <paramref name="numberOfItems" /> items are materialized, or
	///     until the source ends when <paramref name="numberOfItems" /> is <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     A cancellation of the evaluation stops receiving items and leaves the <see cref="ICountable.Count" /> unknown.
	/// </remarks>
	Task<IMaterializedAsyncEnumerable<T>> MaterializeItems(int? numberOfItems);
}
#endif
