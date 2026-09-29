using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Core.EvaluationContext;

internal class EvaluationContext(ExpectationBuilder? expectationBuilder = null) : IEvaluationContext
{
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
}
