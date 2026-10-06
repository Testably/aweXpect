using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Helpers;

internal sealed class MaterializingEnumerable<T> : IEnumerable<T>, ICountable
{
	private readonly IEnumerator<T> _enumerator;
	private readonly List<T> _materializedItems = new();
	private bool _isMaterializedCompletely;
	private Exception? _sourceException;

	private MaterializingEnumerable(IEnumerable<T> enumerable)
	{
		_enumerator = enumerable.GetEnumerator();
	}

	public int? Count { get; private set; }

	/// <summary>
	///     Wraps a sequence that the caller passed as a parameter, e.g. the expected values, whose exceptions propagate
	///     unchanged instead of being reported as if the subject threw them.
	/// </summary>
	/// <remarks>
	///     The subject is materialized by <c>UseMaterializedEnumerable</c> of <c>aweXpect.Core</c> instead.
	/// </remarks>
	public static IEnumerable<T> WrapParameter(IEnumerable<T> enumerable)
	{
		if (enumerable is ICollection<T> or MaterializingEnumerable<T>)
		{
			return enumerable;
		}

		return new MaterializingEnumerable<T>(enumerable);
	}

	/// <remarks>
	///     A source that threw is not advanced again, but every further enumeration throws the same exception, so that
	///     it cannot be mistaken for the end of the source. The source is disposed once it threw or is exhausted; a
	///     source that is only read partially is not disposed, as a later enumeration continues it.
	/// </remarks>
	private bool MoveNext()
	{
		if (_sourceException is not null)
		{
			ExceptionDispatchInfo.Capture(_sourceException).Throw();
		}

		try
		{
			return _enumerator.MoveNext();
		}
		catch (Exception exception)
		{
			_sourceException = exception;
			_enumerator.Dispose();
			throw;
		}
	}

	#region IEnumerable<T> Members

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <remarks>
	///     The items are replayed by index, so that an enumeration nested in another one continues where the other one
	///     stopped, instead of cutting it short.
	/// </remarks>
	public IEnumerator<T> GetEnumerator()
	{
		int index = 0;
		// Stryker disable once Conditional : a mutated condition keeps appending the exhausted enumerator's current item until the test host runs out of memory, which costs a minute per mutant and cannot be killed any cheaper
		while (index < _materializedItems.Count || (!_isMaterializedCompletely && MoveNext()))
		{
			if (index == _materializedItems.Count)
			{
				_materializedItems.Add(_enumerator.Current);
			}

			yield return _materializedItems[index++];
		}

		if (!_isMaterializedCompletely)
		{
			_isMaterializedCompletely = true;
			_enumerator.Dispose();
			Count = _materializedItems.Count;
		}
	}

	#endregion
}
