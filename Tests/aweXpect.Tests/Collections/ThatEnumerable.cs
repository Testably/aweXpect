using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Tests;

public partial class ThatEnumerable
{
	public static IEnumerable<int> ToEnumerable(int[] items)
	{
		foreach (int item in items)
		{
			yield return item;
		}
	}

	public static IEnumerable<string> ToEnumerable(string[] items)
	{
		foreach (string item in items)
		{
			yield return item;
		}
	}

	public static IEnumerable<T> ToEnumerable<T>(int[] items, Func<int, T> mapper)
	{
		foreach (int item in items)
		{
			yield return mapper(item);
		}
	}

	public static IEnumerable<T> ToEnumerable<T>(string[] items, Func<string, T> mapper)
	{
		foreach (string item in items)
		{
			yield return mapper(item);
		}
	}

	public static IEnumerable<T> ToEnumerable<T>(params T[] items)
	{
		foreach (T item in items)
		{
			yield return item;
		}
	}

	/// <summary>
	///     Returns an <see cref="IEnumerable{T}" /> with incrementing numbers, starting with 0, which cancels the
	///     <paramref name="cancellationTokenSource" /> after <paramref name="cancelAfter" /> iteration.
	/// </summary>
	private static IEnumerable<int> GetCancellingEnumerable(
		int cancelAfter,
		CancellationTokenSource cancellationTokenSource,
		int limit = 10_000)
	{
		int index = 0;
		while (index < limit)
		{
			if (index == cancelAfter)
			{
				cancellationTokenSource.Cancel();
			}

			yield return index++;
		}
	}

	/// <summary>
	///     Returns an <see cref="IEnumerable{T}" /> with incrementing numbers, starting with 0, which cancels the
	///     <paramref name="cancellationTokenSource" /> after <paramref name="cancelAfter" /> iteration.
	/// </summary>
	private static IEnumerable<int> GetCancellingEnumerable(
		int[] values,
		int cancelAfter,
		CancellationTokenSource cancellationTokenSource,
		int limit = 10_000)
	{
		int index = 0;
		while (index < limit)
		{
			if (index == cancelAfter)
			{
				cancellationTokenSource.Cancel();
			}

			int idx = index++ % values.Length;
			yield return values[idx];
		}
	}

	/// <summary>
	///     Returns an <see cref="IEnumerable{T}" /> with the <paramref name="items" />, whose enumeration throws the
	///     <paramref name="exception" /> afterwards.
	/// </summary>
	private static IEnumerable<int> ThrowAfter(Exception exception, params int[] items)
	{
		foreach (int item in items)
		{
			yield return item;
		}

		throw exception;
	}

	public sealed class ThrowWhenIteratingTwiceEnumerable : IEnumerable<int>
	{
		private bool _isEnumerated;

		#region IEnumerable<int> Members

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<int> GetEnumerator()
		{
			if (_isEnumerated)
			{
				throw new NotSupportedException("The enumerable was enumerated twice!");
			}

			_isEnumerated = true;
			yield return 1;
		}

		#endregion
	}

	private sealed class Container(IEnumerable<int> items)
	{
		public IEnumerable<int> Items { get; } = items;
	}

	public class InnerClass
	{
		public IEnumerable<string>? Collection { get; set; }

		public InnerClass? Inner { get; set; }
		public string? Value { get; set; }
	}
}
