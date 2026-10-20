#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.TestHelpers;

internal static class AsyncEnumerableExtensions
{
	/// <summary>
	///     Wraps the <paramref name="items" /> in an <see cref="IAsyncEnumerable{T}" />.
	/// </summary>
	public static async IAsyncEnumerable<T> ToTestAsyncEnumerable<T>(this IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			yield return item;
		}

		await Task.CompletedTask;
	}
}
#endif
