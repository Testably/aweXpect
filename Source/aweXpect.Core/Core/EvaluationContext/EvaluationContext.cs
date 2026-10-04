using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext : IEvaluationContext
{
	private EvaluationContext? _attempt;
	private List<AsyncBecauseReason>? _pendingReasons;
	private List<Action>? _releases;
	private Dictionary<string, object?>? _store;

	#region IEvaluationContext Members

	/// <inheritdoc />
	public void Store<T>(string key, T value)
	{
		_store ??= new Dictionary<string, object?>();
		_store[key] = value;
	}

	/// <inheritdoc />
	public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
	{
		if (_store != null &&
		    _store.TryGetValue(key, out object? storedValue)
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
	///     Resolves the reasons registered with <see cref="ResolveOnFailure" /> in this context and in its current
	///     attempt, before the failure message of the evaluation is created.
	/// </summary>
	public async Task ResolvePendingReasons()
	{
		foreach (AsyncBecauseReason reason in _pendingReasons ?? [])
		{
			await reason.Resolve();
		}

		if (_attempt is not null)
		{
			await _attempt.ResolvePendingReasons();
		}
	}

	/// <summary>
	///     Releases the sources of all collections that were materialized in this context and in its current attempt,
	///     and the resources registered with <see cref="ReleaseWithEvaluation" />.
	/// </summary>
	public Task ReleaseMaterializations()
		=> _store is null && _releases is null && _attempt is null
			? Task.CompletedTask
			: ReleaseAll();

	private async Task ReleaseAll()
	{
		foreach (IMaterialization materialization in this.GetMaterializations())
		{
			await materialization.ReleaseSource();
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
		_attempt = new EvaluationContext()
		{
			Cancellation = Cancellation,
		};
		return _attempt;
	}
}
