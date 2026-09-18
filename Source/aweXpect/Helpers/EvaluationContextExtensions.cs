using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Helpers;

/// <summary>
///     Extension methods for the <see cref="IEvaluationContext" />.
/// </summary>
internal static class EvaluationContextExtensions
{
	private const string MaterializedEnumerableKey = nameof(MaterializedEnumerableKey);

	/// <summary>
	///     Avoids enumerating an <see cref="IEnumerable{TItem}" /> multiple times,
	///     by caching already materialized items in the <paramref name="evaluationContext" />.
	/// </summary>
	public static IEnumerable<TItem> UseMaterializedEnumerable<TItem, TCollection>(
		this IEvaluationContext evaluationContext, TCollection collection)
		where TCollection : IEnumerable<TItem>
		=> evaluationContext.GetOrMaterialize(MaterializedEnumerableKey, collection,
			() => MaterializingEnumerable<TItem>.Wrap(collection));

	/// <summary>
	///     Avoids enumerating an <see cref="IEnumerable" /> multiple times,
	///     by caching already materialized items in the <paramref name="evaluationContext" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(collection))]
	public static IEnumerable? UseMaterializedEnumerable<TCollection>(
		this IEvaluationContext evaluationContext, TCollection collection)
		where TCollection : IEnumerable?
	{
		if (collection is null)
		{
			return null;
		}

		return evaluationContext.GetOrMaterialize(MaterializedEnumerableKey, collection,
			() => MaterializingEnumerable.Wrap(collection));
	}

#if NET8_0_OR_GREATER
	private const string MaterializedAsyncEnumerableKey = nameof(MaterializedAsyncEnumerableKey);

	/// <summary>
	///     Avoids enumerating an <see cref="IEnumerable{TItem}" /> multiple times,
	///     by caching already materialized items in the <paramref name="evaluationContext" />.
	/// </summary>
	public static IAsyncEnumerable<TItem> UseMaterializedAsyncEnumerable<TItem, TCollection>(
		this IEvaluationContext evaluationContext, TCollection collection)
		where TCollection : IAsyncEnumerable<TItem>
		=> evaluationContext.GetOrMaterialize(MaterializedAsyncEnumerableKey, collection,
			() => MaterializingAsyncEnumerable<TItem>.Wrap(collection));
#endif

	/// <summary>
	///     Keeps one materialization per source collection, because nested expectations (e.g. <c>ComplyWith</c> on
	///     collection items) evaluate different collections in the same <paramref name="evaluationContext" />.
	/// </summary>
	private static TMaterialized GetOrMaterialize<TMaterialized>(this IEvaluationContext evaluationContext,
		string key, object source, Func<TMaterialized> materialize)
		where TMaterialized : class
	{
		if (!evaluationContext.TryReceive(key, out List<(object Source, object Materialized)>? cache))
		{
			cache = [];
			evaluationContext.Store(key, cache);
		}

		foreach ((object cachedSource, object materialized) in cache)
		{
			if (Equals(cachedSource, source) && materialized is TMaterialized existingValue)
			{
				return existingValue;
			}
		}

		TMaterialized materializedEnumerable = materialize();
		cache.Add((source, materializedEnumerable));
		return materializedEnumerable;
	}
}
