using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     Returns the <paramref name="values" />, throws the <paramref name="exception" /> afterward unless it is
///     <see langword="null" />, and counts how often its enumerator is disposed.
/// </summary>
internal sealed class DisposeTrackingEnumerable(Exception? exception, params int[] values) : IEnumerable<int>
{
	public int DisposeCount { get; private set; }

	public Exception? CurrentException { get; set; }

	public Exception? DisposeException { get; set; }

	public int GetEnumeratorCount { get; private set; }

	public Exception? GetEnumeratorException { get; set; }

	public IEnumerator<int> GetEnumerator()
	{
		GetEnumeratorCount++;
		if (GetEnumeratorException is not null)
		{
			throw GetEnumeratorException;
		}

		return new Enumerator(this, exception, values);
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private sealed class Enumerator(DisposeTrackingEnumerable owner, Exception? exception, int[] values)
		: IEnumerator<int>
	{
		private int _index = -1;

		public int Current => owner.CurrentException is null ? values[_index] : throw owner.CurrentException;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (++_index < values.Length)
			{
				return true;
			}

			if (exception is not null)
			{
				throw exception;
			}

			return false;
		}

		public void Reset() => _index = -1;

		public void Dispose()
		{
			owner.DisposeCount++;
			if (owner.DisposeException is not null)
			{
				throw owner.DisposeException;
			}
		}
	}
}
