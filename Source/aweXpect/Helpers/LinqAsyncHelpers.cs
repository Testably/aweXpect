using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Helpers;

internal static class LinqAsyncHelpers
{
	public static async ValueTask<bool> AnyButNotInDuplicatesAsync<TSource>(
		this IEnumerable<TSource> source,
		IEnumerable<TSource> duplicates,
		Func<TSource, ValueTask<bool>> predicate)
	{
		foreach (TSource item in source)
		{
			if (await predicate(item))
			{
				foreach (TSource duplicate in duplicates)
				{
					if (await predicate(duplicate))
					{
						return false;
					}
				}

				return true;
			}
		}

		return false;
	}
}
