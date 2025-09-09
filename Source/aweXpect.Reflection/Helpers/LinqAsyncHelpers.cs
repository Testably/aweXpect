using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Reflection.Helpers;

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
	
#if NET8_0_OR_GREATER
	public static async Task<(TSource[], TSource[])> SplitAsync<TSource>(
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
			var generated = generator(item);
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
}
