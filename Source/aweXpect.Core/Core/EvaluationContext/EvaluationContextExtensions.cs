using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if NET8_0_OR_GREATER
using System.Threading;
#endif
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     Extension methods for the <see cref="IEvaluationContext" />.
/// </summary>
public static class EvaluationContextExtensions
{
	private const string MaterializedEnumerableKey = nameof(MaterializedEnumerableKey);

	/// <summary>
	///     Avoids enumerating the <paramref name="collection" /> multiple times, by sharing one lazily materialized copy of
	///     it between all constraints that evaluate it in the <paramref name="evaluationContext" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="collection" /> is materialized once per evaluation: every call for the same
	///     <paramref name="collection" /> (compared with <see cref="object.Equals(object, object)" />) in the same
	///     <paramref name="evaluationContext" /> returns the same sequence, also to the built-in collection expectations,
	///     so that expectations combined on one subject (e.g. with <c>.And</c> or <c>.Or</c>) enumerate it at most once.
	///     The items are read from the <paramref name="collection" /> only as far as a consumer enumerates, and every
	///     further enumeration replays them before it continues the source where the previous one stopped. The
	///     enumerator of the source is disposed once the evaluation is completed, including its failure message.<br />
	///     A <paramref name="collection" /> that is an <see cref="ICollection{T}" /> is returned unchanged. Otherwise, the
	///     returned sequence implements <see cref="IMaterializedEnumerable{T}" />, and an exception of the source fails
	///     the expectation like one of <see cref="UserCode" /> and is thrown again by every further enumeration.<br />
	///     The <see cref="IEvaluationContext" /> is only available to a constraint that implements
	///     <see cref="IContextConstraint{TValue}" /> or <see cref="IAsyncContextConstraint{TValue}" />. The returned
	///     sequence must not be enumerated concurrently.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <see langword="null" />.</exception>
	public static IEnumerable<TItem> UseMaterializedEnumerable<TItem>(
		this IEvaluationContext evaluationContext, IEnumerable<TItem> collection)
	{
		collection.ThrowIfNull();
		// A collection that is also non-generic would be returned unchanged by both overloads, so it needs no entry
		// for the other overload to reuse.
		if (collection is ICollection<TItem> and ICollection)
		{
			return collection;
		}

		return evaluationContext.GetOrMaterialize(MaterializedEnumerableKey, collection,
			() => MaterializingEnumerable<TItem>.Wrap(collection));
	}

	/// <summary>
	///     Avoids enumerating the <paramref name="collection" /> multiple times, by sharing one lazily materialized copy of
	///     it between all constraints that evaluate it in the <paramref name="evaluationContext" />.
	/// </summary>
	/// <remarks>
	///     Behaves like <see cref="UseMaterializedEnumerable{TItem}(IEvaluationContext, IEnumerable{TItem})" />, except
	///     that a <see langword="null" /> <paramref name="collection" /> returns <see langword="null" /> and an
	///     <see cref="ICollection" /> is returned unchanged. This overload reuses a sequence that the generic overload
	///     materialized, but not the other way round, so prefer the generic overload when the item type is known.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(collection))]
	public static IEnumerable? UseMaterializedEnumerable(
		this IEvaluationContext evaluationContext, IEnumerable? collection)
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
	///     Avoids enumerating the <paramref name="collection" /> multiple times, by sharing one lazily materialized copy of
	///     it between all constraints that evaluate it in the <paramref name="evaluationContext" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="collection" /> is materialized once per evaluation, like with
	///     <see cref="UseMaterializedEnumerable{TItem}(IEvaluationContext, IEnumerable{TItem})" />, and the returned
	///     sequence implements <see cref="IMaterializedAsyncEnumerable{T}" />.<br />
	///     The <paramref name="cancellationToken" /> of the evaluation is passed to the source and stops waiting for it,
	///     so that a timeout or cancellation also applies to a source that ignores it. As all consumers continue the same
	///     source, it stays governed by the <paramref name="cancellationToken" /> of the call that materialized it; the
	///     token of a later call for the same <paramref name="collection" />, or of a single enumeration, does not
	///     change that. Once the <paramref name="cancellationToken" /> is canceled, the source is not advanced any more,
	///     and an enumeration throws an <see cref="OperationCanceledException" /> after the items received so far.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <see langword="null" />.</exception>
	public static IAsyncEnumerable<TItem> UseMaterializedAsyncEnumerable<TItem>(
		this IEvaluationContext evaluationContext, IAsyncEnumerable<TItem> collection,
		CancellationToken cancellationToken)
	{
		collection.ThrowIfNull();
		return evaluationContext.GetOrMaterialize(MaterializedAsyncEnumerableKey, collection,
			() => MaterializingAsyncEnumerable<TItem>.Wrap(collection, cancellationToken));
	}
#endif

	private static readonly string[] MaterializationKeys =
	[
		MaterializedEnumerableKey,
#if NET8_0_OR_GREATER
		MaterializedAsyncEnumerableKey,
#endif
	];

	/// <summary>
	///     The materializations of the source collections in the <paramref name="evaluationContext" />.
	/// </summary>
	internal static IEnumerable<IMaterialization> GetMaterializations(this IEvaluationContext evaluationContext)
	{
		foreach (string key in MaterializationKeys)
		{
			if (evaluationContext.TryReceive(key, out List<(object Source, object Materialized)>? cache))
			{
				foreach ((object _, object materialized) in cache)
				{
					if (materialized is IMaterialization materialization)
					{
						yield return materialization;
					}
				}
			}
		}
	}

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
