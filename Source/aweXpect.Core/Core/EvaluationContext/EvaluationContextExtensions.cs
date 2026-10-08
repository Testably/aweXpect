using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Signaling;

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
	///     <paramref name="collection" /> (the same instance, or an equal value of a value type) in the same
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
			static source => MaterializingEnumerable<TItem>.Wrap(source));
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
			static source => MaterializingEnumerable.Wrap(source));
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
			(Collection: collection, CancellationToken: cancellationToken),
			static state => MaterializingAsyncEnumerable<TItem>.Wrap(state.Collection, state.CancellationToken));
	}
#endif

	/// <summary>
	///     Gets the current timestamp on the clock of the evaluation, from which
	///     <see cref="GetElapsedTime(IEvaluationContext, long)" /> measures.
	/// </summary>
	/// <remarks>
	///     A constraint that waits measures how long it waited on this clock, on which the timeout of the evaluation
	///     elapses as well, e.g. for <see cref="EvaluationCancellation.HasWaitElapsed(TimeSpan, TimeSpan)" />. The
	///     timestamp is only meaningful for <see cref="GetElapsedTime(IEvaluationContext, long)" /> of the same
	///     <paramref name="evaluationContext" />.
	/// </remarks>
	public static long GetTimestamp(this IEvaluationContext evaluationContext)
		=> EvaluationContext.GetTimeSystem(evaluationContext).GetTimestamp();

	/// <summary>
	///     Gets the time elapsed on the clock of the evaluation since the <paramref name="startTimestamp" /> from
	///     <see cref="GetTimestamp(IEvaluationContext)" />.
	/// </summary>
	public static TimeSpan GetElapsedTime(this IEvaluationContext evaluationContext, long startTimestamp)
		=> EvaluationContext.GetTimeSystem(evaluationContext).GetElapsedTime(startTimestamp);

	/// <summary>
	///     Waits like <see cref="Signaler.WaitAsync(Times, TimeSpan?, CancellationToken)" />, while the
	///     <paramref name="timeout" /> expires on the clock of the evaluation.
	/// </summary>
	/// <remarks>
	///     A constraint waits this way, so that its wait and the timeout of the evaluation are measured on the same
	///     clock, see <see cref="GetTimestamp(IEvaluationContext)" />.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="signaler" /> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="amount" /> is not positive or the <paramref name="timeout" /> is negative.
	/// </exception>
	public static Task<SignalerResult> WaitForSignalsAsync(this IEvaluationContext evaluationContext,
		Signaler signaler, Times amount, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		signaler.ThrowIfNull();
		return signaler.WaitAsync(amount, timeout, EvaluationContext.GetTimeSystem(evaluationContext),
			cancellationToken);
	}

	/// <summary>
	///     Waits like <see cref="Signaler{TParameter}.WaitAsync(Times, Func{TParameter, bool}?, TimeSpan?, CancellationToken)" />,
	///     while the <paramref name="timeout" /> expires on the clock of the evaluation.
	/// </summary>
	/// <remarks>
	///     A constraint waits this way, so that its wait and the timeout of the evaluation are measured on the same
	///     clock, see <see cref="GetTimestamp(IEvaluationContext)" />.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="signaler" /> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	///     The <paramref name="amount" /> is not positive or the <paramref name="timeout" /> is negative.
	/// </exception>
	public static Task<SignalerResult<TParameter>> WaitForSignalsAsync<TParameter>(
		this IEvaluationContext evaluationContext, Signaler<TParameter> signaler, Times amount,
		Func<TParameter, bool>? predicate, TimeSpan? timeout, CancellationToken cancellationToken)
	{
		signaler.ThrowIfNull();
		return signaler.WaitAsync(amount, predicate, timeout, EvaluationContext.GetTimeSystem(evaluationContext),
			cancellationToken);
	}

	private static readonly string[] MaterializationKeys =
	[
		MaterializedEnumerableKey,
#if NET8_0_OR_GREATER
		MaterializedAsyncEnumerableKey,
#endif
	];

	/// <summary>
	///     Whether the <paramref name="key" /> stores materialized collections, which nested evaluations share with the
	///     surrounding evaluation.
	/// </summary>
	internal static bool IsMaterializationKey(string key)
		=> Array.IndexOf(MaterializationKeys, key) >= 0;

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
	private static TMaterialized GetOrMaterialize<TSource, TMaterialized>(this IEvaluationContext evaluationContext,
		string key, TSource source, Func<TSource, TMaterialized> materialize)
		where TSource : class
		where TMaterialized : class
		=> evaluationContext.GetOrMaterialize(key, source, source, materialize);

	/// <inheritdoc cref="GetOrMaterialize{TSource, TMaterialized}(IEvaluationContext, string, TSource, Func{TSource, TMaterialized})" />
	/// <remarks>
	///     The <paramref name="materialize" /> callback receives its values through the <paramref name="state" />, so
	///     that a static callback allocates nothing when the source was already materialized.
	/// </remarks>
	private static TMaterialized GetOrMaterialize<TState, TMaterialized>(this IEvaluationContext evaluationContext,
		string key, object source, TState state, Func<TState, TMaterialized> materialize)
		where TMaterialized : class
	{
		if (!evaluationContext.TryReceive(key, out List<(object Source, object Materialized)>? cache))
		{
			cache = [];
			evaluationContext.Store(key, cache);
		}

		foreach ((object cachedSource, object materialized) in cache)
		{
			if (IsSameSource(cachedSource, source) && materialized is TMaterialized existingValue)
			{
				return existingValue;
			}
		}

		TMaterialized materializedEnumerable = materialize(state);
		cache.Add((source, materializedEnumerable));
		return materializedEnumerable;
	}

	/// <remarks>
	///     A source is identified by its reference, so that two different collections that are equal do not share a
	///     materialization and no <see cref="object.Equals(object)" /> override of the caller is called. A value type is
	///     boxed anew for every call, so it is identified by its value, and one whose equality throws is not known to be
	///     the same.
	/// </remarks>
	private static bool IsSameSource(object cachedSource, object source)
	{
		if (ReferenceEquals(cachedSource, source))
		{
			return true;
		}

		if (source is not ValueType)
		{
			return false;
		}

		try
		{
			return source.Equals(cachedSource);
		}
		catch (Exception)
		{
			return false;
		}
	}
}
