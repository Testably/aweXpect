using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Linq;

#pragma warning disable S3267
internal static class LinqAsyncHelpers
{
#if NET8_0_OR_GREATER
	public static async ValueTask<bool> AllAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
#else
	public static async Task<bool> AllAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, Task<bool>> predicate)
#endif
	{
		foreach (TSource item in source)
		{
			if (!await predicate(item))
			{
				return false;
			}
		}

		return true;
	}

#if NET8_0_OR_GREATER
	public static async ValueTask<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
#else
	public static async Task<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, Task<bool>> predicate)
#endif
	{
		foreach (TSource item in source)
		{
			if (await predicate(item))
			{
				return true;
			}
		}

		return false;
	}

#if NET8_0_OR_GREATER
	public static async ValueTask<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, int, ValueTask<bool>> predicate)
#else
	public static async Task<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, int, Task<bool>> predicate)
#endif
	{
		int index = 0;
		foreach (TSource item in source)
		{
			if (await predicate(item, index++))
			{
				return true;
			}
		}

		return false;
	}

	public static (TSource[], TSource[]) Split<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, bool> predicate)
	{
		List<TSource> matching = [];
		List<TSource> unmatching = [];
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				matching.Add(item);
			}
			else
			{
				unmatching.Add(item);
			}
		}

		return (matching.ToArray(), unmatching.ToArray());
	}

#if NET8_0_OR_GREATER
	public static async ValueTask<(TSource[], TSource[])> SplitAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
#else
	public static async Task<(TSource[], TSource[])> SplitAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, Task<bool>> predicate)
#endif
	{
		List<TSource> matching = [];
		List<TSource> unmatching = [];
		foreach (TSource item in source)
		{
			if (await predicate(item))
			{
				matching.Add(item);
			}
			else
			{
				unmatching.Add(item);
			}
		}

		return (matching.ToArray(), unmatching.ToArray());
	}

#if NET8_0_OR_GREATER
	public static async ValueTask<(TSource[], TSource[])> SplitAsync<TSource>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, bool> predicate)
	{
		List<TSource> matching = [];
		List<TSource> unmatching = [];
		await foreach (TSource item in source)
		{
			if (predicate(item))
			{
				matching.Add(item);
			}
			else
			{
				unmatching.Add(item);
			}
		}

		return (matching.ToArray(), unmatching.ToArray());
	}

	public static async ValueTask<(TSource[], TSource[])> SplitAsync<TSource>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
	{
		List<TSource> matching = [];
		List<TSource> unmatching = [];
		await foreach (TSource item in source)
		{
			if (await predicate(item))
			{
				matching.Add(item);
			}
			else
			{
				unmatching.Add(item);
			}
		}

		return (matching.ToArray(), unmatching.ToArray());
	}
#endif

#if NET8_0_OR_GREATER
	public static async Task<(TSource[], TSource[])> SplitWhereAnyAsync<TSource, TTarget>(
		this IEnumerable<TSource> source,
		Func<TSource, IEnumerable<TTarget>?> generator,
		Func<TTarget, ValueTask<bool>> predicate)
#else
	public static async Task<(TSource[], TSource[])> SplitWhereAnyAsync<TSource, TTarget>(
		this IEnumerable<TSource> source,
		Func<TSource, IEnumerable<TTarget>?> generator,
		Func<TTarget, Task<bool>> predicate)
#endif
	{
		List<TSource> matching = [];
		List<TSource> unmatching = [];
		foreach (TSource item in source)
		{
			IEnumerable<TTarget>? generated = generator(item);
			if (generated is not null && await generated.AnyAsync(predicate))
			{
				matching.Add(item);
			}
			else
			{
				unmatching.Add(item);
			}
		}

		return (matching.ToArray(), unmatching.ToArray());
	}

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TSource> Where<TSource>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, bool> predicate)
	{
		await foreach (TSource item in source)
		{
			if (predicate(item))
			{
				yield return item;
			}
		}
	}
#endif

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TSource> WhereNotNull<TSource>(
		this IAsyncEnumerable<TSource?> source)
	{
		await foreach (TSource? item in source)
		{
			if (item is not null)
			{
				yield return item;
			}
		}
	}

	public static async IAsyncEnumerable<TSource> WhereNotNull<TSource>(
		this IEnumerable<TSource?> source)
	{
		await Task.Yield();
		foreach (TSource? item in source)
		{
			if (item is not null)
			{
				yield return item;
			}
		}
	}
#else
	public static IEnumerable<TSource> WhereNotNull<TSource>(
		this IEnumerable<TSource?> source)
	{
		foreach (TSource? item in source)
		{
			if (item is not null)
			{
				yield return item;
			}
		}
	}
#endif

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, TResult> selector)
	{
		await foreach (TSource item in source)
		{
			yield return selector(item);
		}
	}
#endif

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, IEnumerable<TResult>> selector)
	{
		await foreach (TSource item in source)
		{
			foreach (TResult result in selector(item))
			{
				yield return result;
			}
		}
	}
#endif

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TSource> Distinct<TSource>(
		this IAsyncEnumerable<TSource> source)
	{
		HashSet<TSource> hashSet = new();
		await foreach (TSource item in source)
		{
			if (hashSet.Add(item))
			{
				yield return item;
			}
		}
	}
#endif
}
#pragma warning restore S3267
