using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext(ExpectationBuilder? expectationBuilder = null) : IEvaluationContext
{
	private EvaluationContext? _attempt;
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

	#endregion

	/// <summary>
	///     Adds the <paramref name="otherExceptions" /> of a faulted task to the context of the expectation that is
	///     evaluated, unless they are <see langword="null" />.
	/// </summary>
	public void AddOtherExceptions(Exception[]? otherExceptions)
		=> expectationBuilder?.AddOtherExceptions(otherExceptions);

	/// <summary>
	///     Releases the sources of all collections that were materialized in this context and in its current attempt.
	/// </summary>
	public async Task ReleaseMaterializations()
	{
		foreach (IMaterialization materialization in this.GetMaterializations())
		{
			await materialization.ReleaseSource();
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
		_attempt = new EvaluationContext(expectationBuilder);
		return _attempt;
	}
}
