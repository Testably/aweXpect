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

	public Exception? DisposeException { get; set; }

	public IEnumerator<int> GetEnumerator() => new Enumerator(this, exception, values);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private sealed class Enumerator(DisposeTrackingEnumerable owner, Exception? exception, int[] values)
		: IEnumerator<int>
	{
		private int _index = -1;

		public int Current => values[_index];

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
