using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Threading;
#endif
using System.Threading.Tasks;

namespace aweXpect.Reflection.Helpers;

#pragma warning disable S3267
internal static class LinqAsyncHelpers
{
	public static async ValueTask<bool> AllAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
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

	public static async ValueTask<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, ValueTask<bool>> predicate)
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

	public static async ValueTask<bool> AnyAsync<TSource>(
		this IEnumerable<TSource> source,
		Func<TSource, int, ValueTask<bool>> predicate)
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

	public static async ValueTask<(TSource[], TSource[])> SplitWhereAnyAsync<TSource, TTarget>(
		this IEnumerable<TSource> source,
		Func<TSource, IEnumerable<TTarget>?> generator,
		Func<TTarget, ValueTask<bool>> predicate)
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
		Func<TSource, bool> predicate,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (TSource item in source.WithCancellation(cancellationToken))
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
		this IAsyncEnumerable<TSource?> source,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (TSource? item in source.WithCancellation(cancellationToken))
		{
			if (item is not null)
			{
				yield return item;
			}
		}
	}

	public static async IAsyncEnumerable<TSource> WhereNotNull<TSource>(
		this IEnumerable<TSource?> source,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (TSource? item in source)
		{
			cancellationToken.ThrowIfCancellationRequested();
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

#if NET8_0
	public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, TResult> selector,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (TSource item in source.WithCancellation(cancellationToken))
		{
			yield return selector(item);
		}
	}
#endif

#if NET8_0
	public static async IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, IEnumerable<TResult>> selector,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (TSource item in source.WithCancellation(cancellationToken))
		{
			foreach (TResult result in selector(item))
			{
				yield return result;
			}
		}
	}
#endif

#if NET8_0_OR_GREATER
	public static async IAsyncEnumerable<TResult> SelectManyNotNull<TSource, TResult>(
		this IEnumerable<TSource?> source,
		Func<TSource, IEnumerable<TResult>> selector,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (TSource? item in source)
		{
			if (item is null)
			{
				continue;
			}

			foreach (TResult result in selector(item))
			{
				cancellationToken.ThrowIfCancellationRequested();
				yield return result;
			}
		}
	}
#else
	public static IEnumerable<TResult> SelectManyNotNull<TSource, TResult>(
		this IEnumerable<TSource?> source,
		Func<TSource, IEnumerable<TResult>> selector)
	{
		foreach (TSource? item in source)
		{
			if (item is null)
			{
				continue;
			}

			foreach (TResult result in selector(item))
			{
				yield return result;
			}
		}
	}
#endif

#if NET8_0
	public static async IAsyncEnumerable<TSource> Distinct<TSource>(
		this IAsyncEnumerable<TSource> source,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		HashSet<TSource> hashSet = new();
		await foreach (TSource item in source.WithCancellation(cancellationToken))
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
