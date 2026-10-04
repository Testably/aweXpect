using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext : IEvaluationContext
{
	private EvaluationContext? _attempt;
	private List<EvaluationContext>? _checks;
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
		_attempt = new EvaluationContext()
		{
			Cancellation = Cancellation,
		};
		return _attempt;
	}
}
