using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext : IEvaluationContext
{
	private Extras? _extras;
	private int _nestingDepth;
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
		=> _extras?.NestedStores is { } nestedStores && nestedStores.Count >= _nestingDepth
			? nestedStores[_nestingDepth - 1]
			: null;

	/// <summary>
	///     Registers the <paramref name="release" /> of a resource that the evaluation, including its failure message,
	///     still uses, so that it is released together with the materialized sources of this context.
	/// </summary>
	/// <remarks>
	///     The context of a check of a repeated check registers it in the context that started the check, because the
	///     evaluation goes on with the next check, which still uses the resource.
	/// </remarks>
	public void ReleaseWithEvaluation(Action release)
	{
		if (_extras?.RepeatedBy is { } repeatedBy)
		{
			repeatedBy.ReleaseWithEvaluation(release);
			return;
		}

		(GetExtras().Releases ??= []).Add(release);
	}

	/// <summary>
	///     Registers the <paramref name="reason" /> of a met expectation, which the failure message still shows when an
	///     outer negation or combination fails the evaluation, so that <see cref="ResolvePendingReasons" /> resolves it.
	/// </summary>
	public void ResolveOnFailure(AsyncBecauseReason reason)
	{
		List<AsyncBecauseReason> pendingReasons = GetExtras().PendingReasons ??= [];
		if (!pendingReasons.Contains(reason))
		{
			pendingReasons.Add(reason);
		}
	}

	/// <summary>
	///     Whether a reason is registered with <see cref="ResolveOnFailure" /> in this context or in its current
	///     attempt.
	/// </summary>
	public bool HasPendingReasons
		=> _extras is not null &&
		   (_extras.PendingReasons is not null || _extras.Attempt is { HasPendingReasons: true, });

	/// <summary>
	///     Resolves the reasons registered with <see cref="ResolveOnFailure" /> in this context and in its current
	///     attempt, before the failure message of the evaluation is created.
	/// </summary>
	/// <remarks>
	///     A reason that is still pending when the <paramref name="cancellationToken" /> is canceled is abandoned.
	/// </remarks>
	public async Task ResolvePendingReasons(CancellationToken cancellationToken)
	{
		foreach (AsyncBecauseReason reason in _extras?.PendingReasons ?? [])
		{
			await reason.Resolve(cancellationToken);
		}

		if (_extras?.Attempt is { } attempt)
		{
			await attempt.ResolvePendingReasons(cancellationToken);
		}
	}

	/// <summary>
	///     Releases the sources of all collections that were materialized in this context, in its current attempt and in
	///     the last check of its repeated checks, and the resources registered with <see cref="ReleaseWithEvaluation" />.
	/// </summary>
	public Task ReleaseMaterializations()
		=> _store is null && _extras is null
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

		if (_extras is null)
		{
			return;
		}

		if (_extras.Releases is { } releases)
		{
			_extras.Releases = null;
			foreach (Action release in releases)
			{
				release();
			}
		}

		if (_extras.Attempt is { } attempt)
		{
			await attempt.ReleaseMaterializations();
		}

		if (_extras.Checks is { } checks)
		{
			_extras.Checks = null;
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
			_extras = new Extras
			{
				RepeatedBy = this,
			},
		};
		List<EvaluationContext> checks = GetExtras().Checks ??= [];
		if (previous is null)
		{
			checks.Add(check);
		}
		else
		{
			checks[checks.IndexOf(previous)] = check;
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
		_extras?.Attempt?.ForwardCompletionCallbacksTo(this);
		EvaluationContext attempt = new()
		{
			Cancellation = Cancellation,
			TimeSystem = TimeSystem,
		};
		GetExtras().Attempt = attempt;
		return attempt;
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
			List<Dictionary<string, object?>?> nestedStores = GetExtras().NestedStores ??= [];
			while (nestedStores.Count < _nestingDepth)
			{
				nestedStores.Add(null);
			}

			(nestedStores[_nestingDepth - 1] ??= new Dictionary<string, object?>())[key] = value;
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

	/// <inheritdoc />
	/// <remarks>
	///     The context of a check of a repeated check registers the <paramref name="callback" /> in the context that
	///     started the check, as the check ends before the evaluation does.
	/// </remarks>
	public void OnCompleted(Func<CompletedEvaluation, CancellationToken, ValueTask> callback)
	{
		callback.ThrowIfNull();
		if (_extras?.RepeatedBy is { } repeatedBy)
		{
			repeatedBy.OnCompleted(callback);
			return;
		}

		Extras extras = GetExtras();
		List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? callbacks = extras.CompletionCallbacks;
		CompletionCallbacks.Add(ref callbacks, callback);
		extras.CompletionCallbacks = callbacks;
	}

	#endregion

	/// <summary>
	///     Whether a callback is registered with <see cref="OnCompleted" /> in this context or in its current attempt.
	/// </summary>
	public bool HasCompletionCallbacks
		=> _extras is not null &&
		   (_extras.CompletionCallbacks is not null || _extras.Attempt is { HasCompletionCallbacks: true, });

	/// <summary>
	///     Adds the callbacks registered with <see cref="OnCompleted" /> in this context and in its current attempt to the
	///     <paramref name="callbacks" />.
	/// </summary>
	public void CollectCompletionCallbacks(
		ref List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? callbacks)
	{
		if (_extras is null)
		{
			return;
		}

		if (_extras.CompletionCallbacks is { } completionCallbacks)
		{
			foreach (Func<CompletedEvaluation, CancellationToken, ValueTask> callback in completionCallbacks)
			{
				CompletionCallbacks.Add(ref callbacks, callback);
			}
		}

		_extras.Attempt?.CollectCompletionCallbacks(ref callbacks);
	}

	/// <summary>
	///     Registers the callbacks of this context with <see cref="OnCompleted" /> in the <paramref name="context" />, in
	///     which the evaluation that this context was part of completes.
	/// </summary>
	public void ForwardCompletionCallbacksTo(IEvaluationContext context)
	{
		if (!HasCompletionCallbacks)
		{
			return;
		}

		List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? callbacks = null;
		CollectCompletionCallbacks(ref callbacks);
		foreach (Func<CompletedEvaluation, CancellationToken, ValueTask> callback in callbacks!)
		{
			context.OnCompleted(callback);
		}
	}

	/// <summary>
	///     Runs the callbacks registered with <see cref="OnCompleted" /> with the <paramref name="outcome" /> of the whole
	///     expectation, skipping and extending the <paramref name="ran" /> ones of other evaluations in the same
	///     combination.
	/// </summary>
	/// <remarks>
	///     The callbacks receive the token of the caller, as the token of the evaluation is already canceled after a
	///     timeout.
	/// </remarks>
	public Task RunCompletionCallbacks(Outcome outcome,
		List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? ran = null)
		=> RunCompletionCallbacks(outcome, Cancellation.CallerToken, ran);

	/// <summary>
	///     Runs the callbacks like <see cref="RunCompletionCallbacks(Outcome, List{Func{CompletedEvaluation, CancellationToken, ValueTask}})" />,
	///     with the <paramref name="callerToken" /> of an evaluation that has no cancellation of its own.
	/// </summary>
	public Task RunCompletionCallbacks(Outcome outcome, CancellationToken callerToken,
		List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? ran = null)
	{
		if (!HasCompletionCallbacks)
		{
			return Task.CompletedTask;
		}

		List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? callbacks = null;
		CollectCompletionCallbacks(ref callbacks);
		if (ran is not null)
		{
			callbacks!.RemoveAll(ran.Contains);
			ran.AddRange(callbacks);
		}

		return CompletionCallbacks.Run(callbacks!, outcome, callerToken);
	}

	private Extras GetExtras() => _extras ??= new Extras();

	/// <summary>
	///     The state of the repeated checks, the attempts, the reasons to await, the resources to release, the
	///     values stored during nested evaluations and the callbacks to run when the evaluation completes.
	/// </summary>
	/// <remarks>
	///     A context is created for every expectation, while most expectations need none of it, so it is only created
	///     when it is first used.
	/// </remarks>
	private sealed class Extras
	{
		public EvaluationContext? Attempt { get; set; }
		public List<EvaluationContext>? Checks { get; set; }
		public List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? CompletionCallbacks { get; set; }
		public List<Dictionary<string, object?>?>? NestedStores { get; set; }
		public List<AsyncBecauseReason>? PendingReasons { get; set; }
		public List<Action>? Releases { get; set; }
		public EvaluationContext? RepeatedBy { get; init; }
	}
}
