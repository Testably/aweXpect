using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext : IEvaluationContext
{
	private EvaluationContext? _attempt;
	private List<EvaluationContext>? _checks;
	private List<Dictionary<string, object?>?>? _nestedStores;
	private int _nestingDepth;
	private List<AsyncBecauseReason>? _pendingReasons;
	private List<Action>? _releases;
	private Dictionary<string, object?>? _store;

	/// <summary>
	///     The time system of the evaluation, with which a repeated check measures the time and waits between its checks.
	/// </summary>
	public ITimeSystem TimeSystem { get; set; } = RealTimeSystem.Instance;

	/// <summary>
	///     Starts the evaluation of an item or a member in the <paramref name="context" />, whose stored values are
	///     separate from those of the surrounding evaluation and are forgotten when the returned scope is disposed.
	/// </summary>
	/// <remarks>
	///     The materialized collections, the reasons and the resources to release stay shared with the surrounding
	///     evaluation. Nothing is allocated unless a value is stored during the nested evaluation.
	/// </remarks>
	public static NestedEvaluation StartNestedEvaluation(IEvaluationContext context)
		=> new(context as EvaluationContext);

	/// <summary>
	///     Returns a new context for an expectation that is evaluated on its own during the evaluation in the
	///     <paramref name="context" />, e.g. an <c>It.Is…</c> in an expected object: it is canceled and measures the
	///     time like that evaluation, while its materialized collections, reasons and stored values are its own.
	/// </summary>
	/// <remarks>
	///     The caller releases the returned context. Without a <paramref name="context" />, i.e. outside an evaluation,
	///     it neither times out nor is canceled.
	/// </remarks>
	public static EvaluationContext ForNestedExpectation(IEvaluationContext? context)
		=> context is null
			? new EvaluationContext()
			: new EvaluationContext
			{
				Cancellation = context.Cancellation,
				TimeSystem = GetTimeSystem(context),
			};

	/// <summary>
	///     The time system of the evaluation in the <paramref name="context" />, or the real one outside an evaluation
	///     and for a <paramref name="context" /> of another implementation.
	/// </summary>
	public static ITimeSystem GetTimeSystem(IEvaluationContext? context)
		=> context is EvaluationContext evaluationContext
			? evaluationContext.TimeSystem
			: RealTimeSystem.Instance;

	private bool IsNested(string key)
		=> _nestingDepth > 0 && !EvaluationContextExtensions.IsMaterializationKey(key);

	private Dictionary<string, object?>? GetNestedStore()
		=> _nestedStores is not null && _nestedStores.Count >= _nestingDepth
			? _nestedStores[_nestingDepth - 1]
			: null;

	/// <summary>
	///     Registers the <paramref name="release" /> of a resource that the evaluation, including its failure message,
	///     still uses, so that it is released together with the materialized sources of this context.
	/// </summary>
	public void ReleaseWithEvaluation(Action release)
		=> (_releases ??= []).Add(release);

	/// <summary>
	///     Registers the <paramref name="reason" /> of a met expectation, which the failure message still shows when an
	///     outer negation or combination fails the evaluation, so that <see cref="ResolvePendingReasons" /> resolves it.
	/// </summary>
	public void ResolveOnFailure(AsyncBecauseReason reason)
	{
		_pendingReasons ??= [];
		if (!_pendingReasons.Contains(reason))
		{
			_pendingReasons.Add(reason);
		}
	}

	/// <summary>
	///     Whether a reason is registered with <see cref="ResolveOnFailure" /> in this context or in its current
	///     attempt.
	/// </summary>
	public bool HasPendingReasons => _pendingReasons is not null || _attempt is { HasPendingReasons: true, };

	/// <summary>
	///     Resolves the reasons registered with <see cref="ResolveOnFailure" /> in this context and in its current
	///     attempt, before the failure message of the evaluation is created.
	/// </summary>
	/// <remarks>
	///     A reason that is still pending when the <paramref name="cancellationToken" /> is canceled is abandoned.
	/// </remarks>
	public async Task ResolvePendingReasons(CancellationToken cancellationToken)
	{
		foreach (AsyncBecauseReason reason in _pendingReasons ?? [])
		{
			await reason.Resolve(cancellationToken);
		}

		if (_attempt is not null)
		{
			await _attempt.ResolvePendingReasons(cancellationToken);
		}
	}

	/// <summary>
	///     Releases the sources of all collections that were materialized in this context, in its current attempt and in
	///     the last check of its repeated checks, and the resources registered with <see cref="ReleaseWithEvaluation" />.
	/// </summary>
	public Task ReleaseMaterializations()
		=> _store is null && _releases is null && _attempt is null && _checks is null
			? Task.CompletedTask
			: ReleaseAll();

	private async Task ReleaseAll()
	{
		foreach (IMaterialization materialization in this.GetMaterializations())
		{
			Task release = materialization.ReleaseSource();
			if (!release.IsCompleted)
			{
				await AwaitOrAbandon(release);
			}
		}

		if (_releases is not null)
		{
			List<Action> releases = _releases;
			_releases = null;
			foreach (Action release in releases)
			{
				release();
			}
		}

		if (_attempt is not null)
		{
			await _attempt.ReleaseMaterializations();
		}

		if (_checks is not null)
		{
			List<EvaluationContext> checks = _checks;
			_checks = null;
			foreach (EvaluationContext check in checks)
			{
				await check.ReleaseMaterializations();
			}
		}
	}

	/// <summary>
	///     Awaits the <paramref name="release" /> of a source until the timeout of the evaluation elapses or the
	///     evaluation is canceled, and abandons it then.
	/// </summary>
	/// <remarks>
	///     The outcome is already decided when a source is released, so a source that does not finish disposing must
	///     neither change it nor keep the evaluation from ending.
	/// </remarks>
	private async Task AwaitOrAbandon(Task release)
	{
		EvaluationCancellation cancellation = Cancellation.ForRemainingTimeout();
		using EvaluationCancellation.ReleaseScope _ = cancellation.ReleaseAtTheEnd();
		try
		{
			await release.AbandonOnCancellation(cancellation.Token);
		}
		catch (OperationCanceledException) when (cancellation.Token.IsCancellationRequested)
		{
			// The source keeps disposing on its own.
		}
	}

	/// <summary>
	///     Returns a new context for another check of a repeated check, so that it reads the collections again, and
	///     releases the sources of the <paramref name="previous" /> check of the same repeated check.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="StartAttempt" />, the sources of this context are kept, because the other expectations of
	///     the evaluation still use them. The sources of the last check are released together with this context.
	/// </remarks>
	public async Task<EvaluationContext> StartCheck(EvaluationContext? previous)
	{
		EvaluationContext check = new()
		{
			Cancellation = Cancellation,
			TimeSystem = TimeSystem,
		};
		_checks ??= [];
		if (previous is null)
		{
			_checks.Add(check);
		}
		else
		{
			_checks[_checks.IndexOf(previous)] = check;
			await previous.ReleaseMaterializations();
		}

		return check;
	}

	/// <summary>
	///     Releases the sources materialized so far and returns a new context for another attempt to meet the
	///     expectations, whose sources are released together with this context.
	/// </summary>
	/// <remarks>
	///     Each attempt is a separate evaluation with its own context.
	/// </remarks>
	public async Task<EvaluationContext> StartAttempt()
	{
		await ReleaseMaterializations();
		_attempt = new EvaluationContext
		{
			Cancellation = Cancellation,
			TimeSystem = TimeSystem,
		};
		return _attempt;
	}

	/// <summary>
	///     The scope of a nested evaluation, see <see cref="StartNestedEvaluation" />.
	/// </summary>
	internal readonly struct NestedEvaluation : IDisposable
	{
		private readonly EvaluationContext? _context;

		public NestedEvaluation(EvaluationContext? context)
		{
			_context = context;
			if (context is not null)
			{
				context._nestingDepth++;
			}
		}

		public void Dispose()
		{
			if (_context is null)
			{
				return;
			}

			_context.GetNestedStore()?.Clear();
			_context._nestingDepth--;
		}
	}

	#region IEvaluationContext Members

	/// <inheritdoc />
	/// <remarks>
	///     A value stored while an item or a member is evaluated is only received during that evaluation.
	/// </remarks>
	public void Store<T>(string key, T value)
	{
		if (IsNested(key))
		{
			_nestedStores ??= [];
			while (_nestedStores.Count < _nestingDepth)
			{
				_nestedStores.Add(null);
			}

			(_nestedStores[_nestingDepth - 1] ??= new Dictionary<string, object?>())[key] = value;
			return;
		}

		_store ??= new Dictionary<string, object?>();
		_store[key] = value;
	}

	/// <inheritdoc />
	public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
	{
		Dictionary<string, object?>? store = IsNested(key) ? GetNestedStore() : _store;
		if (store != null &&
		    store.TryGetValue(key, out object? storedValue)
		    && storedValue is T typeMatchingValue)
		{
			value = typeMatchingValue;
			return true;
		}

		value = default;
		return false;
	}

	/// <inheritdoc />
	public EvaluationCancellation Cancellation { get; set; } = EvaluationCancellation.None;

	#endregion
}
