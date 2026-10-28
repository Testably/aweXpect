using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext : IEvaluationContext
{
	/// <summary>
	///     A context of this thread that is not in use and kept nothing, see <see cref="Rent" />.
	/// </summary>
	[ThreadStatic] private static EvaluationContext? _unused;

	private Extras? _extras;
	private int _nestingDepth;

	/// <summary>
	///     The values stored outside a nested evaluation.
	/// </summary>
	/// <remarks>
	///     An array searched linearly instead of a dictionary, as an evaluation stores only a few values, mostly the
	///     materialized collections, and a dictionary costs more than the materialization itself.
	/// </remarks>
	private (string Key, object? Value)[]? _store;

	private int _storeCount;

	/// <summary>
	///     The clock of the evaluation, with which a repeated check measures the time and waits between its checks.
	/// </summary>
	/// <remarks>
	///     Only another clock than the real one is kept in the <see cref="Extras" />, so that the context of most
	///     expectations stays smaller.
	/// </remarks>
	public TimeProvider TimeProvider
	{
		get => _extras?.TimeProvider ?? TimeProvider.System;
		set
		{
			if (_extras is not null || !ReferenceEquals(value, TimeProvider.System))
			{
				GetExtras().TimeProvider = value;
			}
		}
	}

	/// <summary>
	///     Whether nothing was stored or registered in the context, so that nothing refers to it after the evaluation.
	/// </summary>
	public bool IsUnused => _extras is null && _storeCount == 0 && _nestingDepth == 0;

	/// <summary>
	///     Returns an unused context for an evaluation that most likely completes right away.
	/// </summary>
	/// <remarks>
	///     Pass it to <see cref="Return" /> when the evaluation is done, so that the next one need not allocate it.
	/// </remarks>
	public static EvaluationContext Rent()
	{
		EvaluationContext? context = _unused;
		if (context is null)
		{
			return new EvaluationContext();
		}

		_unused = null;
		return context;
	}

	/// <summary>
	///     Keeps the <paramref name="context" /> for the next <see cref="Rent" />, when it is still unused.
	/// </summary>
	public static void Return(EvaluationContext context)
	{
		if (context.IsUnused)
		{
			context.Cancellation = EvaluationCancellation.None;
			_unused = context;
		}
	}

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
				TimeProvider = GetTimeProvider(context),
			};

	/// <summary>
	///     The clock of the evaluation in the <paramref name="context" />, or the real one outside an evaluation
	///     and for a <paramref name="context" /> of another implementation.
	/// </summary>
	public static TimeProvider GetTimeProvider(IEvaluationContext? context)
		=> context is EvaluationContext evaluationContext
			? evaluationContext.TimeProvider
			: TimeProvider.System;

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
	///     Adds the <paramref name="formatters" /> of a nested expectation to the evaluation in the
	///     <paramref name="context" />, so that its failure message, which renders the nested expectation, uses them.
	/// </summary>
	/// <remarks>
	///     A context of another implementation, or the text context of no evaluation, keeps nothing.
	/// </remarks>
	public static void AddFormatters(IEvaluationContext context, IValueFormatter[] formatters)
	{
		EvaluationContext? evaluation = context as EvaluationContext ??
		                                (context as ExpectationTextEvaluationContext)?.Evaluation;
		evaluation?.AddFormatters(formatters);
	}

	private void AddFormatters(IValueFormatter[] formatters)
	{
		if (_extras?.RepeatedBy is { } repeatedBy)
		{
			repeatedBy.AddFormatters(formatters);
			return;
		}

		List<IValueFormatter> ownFormatters = GetExtras().Formatters ??= [];
		foreach (IValueFormatter formatter in formatters)
		{
			if (!ownFormatters.Contains(formatter))
			{
				ownFormatters.Add(formatter);
			}
		}
	}

	/// <summary>
	///     Adds the formatters that nested expectations added in this context and in its current attempt to the
	///     <paramref name="formatters" />.
	/// </summary>
	public void CollectFormatters(List<IValueFormatter> formatters)
	{
		if (_extras is null)
		{
			return;
		}

		foreach (IValueFormatter formatter in _extras.Formatters ?? [])
		{
			if (!formatters.Contains(formatter))
			{
				formatters.Add(formatter);
			}
		}

		_extras.Attempt?.CollectFormatters(formatters);
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
		await ReleaseMaterializedSources();

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

	private async Task ReleaseMaterializedSources()
	{
		foreach (string key in EvaluationContextExtensions.MaterializationKeys)
		{
			if (!TryReceive(key, out MaterializationCache? cache))
			{
				continue;
			}

			for (int i = 0; i < cache.Count; i++)
			{
				if (cache.GetMaterialized(i) is not IMaterialization materialization)
				{
					continue;
				}

				Task release = materialization.ReleaseSource();
				if (!release.IsCompleted)
				{
					await AwaitOrAbandon(release);
				}
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
			_extras = new Extras
			{
				RepeatedBy = this,
				TimeProvider = _extras?.TimeProvider,
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
			TimeProvider = TimeProvider,
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

		int index = IndexOfStoredKey(key);
		if (index >= 0)
		{
			_store![index].Value = value;
			return;
		}

		if (_store is null)
		{
			_store = new (string, object?)[2];
		}
		else if (_storeCount == _store.Length)
		{
			Array.Resize(ref _store, _storeCount * 2);
		}

		_store[_storeCount++] = (key, value);
	}

	/// <inheritdoc />
	public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
	{
		object? storedValue = null;
		bool isStored;
		if (IsNested(key))
		{
			Dictionary<string, object?>? nestedStore = GetNestedStore();
			isStored = nestedStore is not null && nestedStore.TryGetValue(key, out storedValue);
		}
		else
		{
			int index = _store is null ? -1 : IndexOfStoredKey(key);
			isStored = index >= 0;
			storedValue = isStored ? _store![index].Value : null;
		}

		if (isStored && storedValue is T typeMatchingValue)
		{
			value = typeMatchingValue;
			return true;
		}

		value = default;
		return false;
	}

	/// <remarks>
	///     A <see langword="null" /> <paramref name="key" /> throws, like for the nested stores.
	/// </remarks>
	private int IndexOfStoredKey(string key)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		for (int i = 0; i < _storeCount; i++)
		{
			if (string.Equals(_store![i].Key, key, StringComparison.Ordinal))
			{
				return i;
			}
		}

		return -1;
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
	///     values stored during nested evaluations, the formatters of nested expectations and the callbacks to run when
	///     the evaluation completes.
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
		public List<IValueFormatter>? Formatters { get; set; }
		public List<Dictionary<string, object?>?>? NestedStores { get; set; }
		public List<AsyncBecauseReason>? PendingReasons { get; set; }
		public List<Action>? Releases { get; set; }
		public EvaluationContext? RepeatedBy { get; init; }
		public TimeProvider? TimeProvider { get; set; }
	}
}
