using System.Collections.Generic;
using System.Linq;
using aweXpect.Reflection.Helpers;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Threading;
#endif

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class LinqAsyncHelpersTests
{
	[Test]
	public async Task SelectManyNotNull_ShouldSkipNullItemsAndKeepTheOrder()
	{
		string?[] source = ["ab", null, "c",];

		List<char> result = [];
#if NET8_0_OR_GREATER
		await foreach (char item in source.SelectManyNotNull(s => s))
#else
		foreach (char item in source.SelectManyNotNull(s => s))
#endif
		{
			result.Add(item);
		}

		await That(result).IsEqualTo(['a', 'b', 'c',]);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task SelectManyNotNull_WhenCanceled_ShouldStopTheEnumeration()
	{
		using CancellationTokenSource cts = new();
		string?[] source = ["ab", null, "c",];
		List<char> result = [];

		async Task Act()
		{
			await foreach (char item in source.SelectManyNotNull(s => s).WithCancellation(cts.Token))
			{
				result.Add(item);
				cts.Cancel();
			}
		}

		await That(Act).Throws<OperationCanceledException>().WithMessage("The operation was canceled.");
		await That(result).IsEqualTo(['a',]);
	}
#endif

	[Test]
	public async Task SplitWhereAnyAsync_ShouldSeparateBasedOnGeneratedItems()
	{
		int[] source = [1, 2, 3,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			i => Enumerable.Range(0, i),
			target => new ValueTask<bool>(target > 0));

		// generator(1) => [0]            => no value > 0 => not matching
		// generator(2) => [0, 1]         => contains 1   => matching
		// generator(3) => [0, 1, 2]      => contains 1,2 => matching
		await That(matching).IsEqualTo([2, 3,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1,]).InAnyOrder();
	}

	[Test]
	public async Task SplitWhereAnyAsync_WhenGeneratorReturnsNull_ShouldNotMatch()
	{
		int[] source = [1, 2,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			i => i == 1 ? null : Enumerable.Range(0, 5),
			target => new ValueTask<bool>(target >= 0));

		// generator(1) => null => not matching (must not call AnyAsync on null)
		// generator(2) => [0..4] => contains values >= 0 => matching
		await That(matching).IsEqualTo([2,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1,]).InAnyOrder();
	}

	[Test]
	public async Task SplitWhereAnyAsync_WhenGeneratorYieldsNoMatch_ShouldNotMatch()
	{
		int[] source = [1, 2,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			_ => Enumerable.Range(0, 3),
			target => new ValueTask<bool>(target > 100));

		// No generated value is > 100 => nothing matches.
		await That(matching).IsEmpty();
		await That(notMatching).IsEqualTo([1, 2,]).InAnyOrder();
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task Where_ShouldYieldOnlyMatchingItems()
	{
		int[] source = [1, 2, 3, 4,];

		List<int> result = new();
		await foreach (int item in LinqAsyncHelpers.Where(ToAsync(source), i => i % 2 == 0))
		{
			result.Add(item);
		}

		await That(result).IsEqualTo([2, 4,]).InAnyOrder();
	}

	[Test]
	public async Task Where_WhenCanceled_ShouldPassTheTokenToTheSource()
	{
		using CancellationTokenSource cts = new();
		int[] source = [1, 2, 3, 4,];
		List<int> result = [];

		async Task Act()
		{
			await foreach (int item in LinqAsyncHelpers.Where(ToAsync(source), _ => true).WithCancellation(cts.Token))
			{
				result.Add(item);
				cts.Cancel();
			}
		}

		await That(Act).Throws<OperationCanceledException>().WithMessage("The operation was canceled.");
		await That(result).IsEqualTo([1,]);
	}

	[Test]
	public async Task WhereNotNull_FromEnumerable_ShouldYieldOnlyNonNullItems()
	{
		int?[] source = [1, null, 2, null, 3,];

		List<int> result = new();
		await foreach (int? item in source.WhereNotNull())
		{
			result.Add(item!.Value);
		}

		await That(result).IsEqualTo([1, 2, 3,]).InAnyOrder();
	}

	[Test]
	public async Task WhereNotNull_FromEnumerable_WhenCanceled_ShouldStopTheEnumeration()
	{
		using CancellationTokenSource cts = new();
		int?[] source = [1, null, 2, 3,];
		List<int> result = [];

		async Task Act()
		{
			await foreach (int? item in source.WhereNotNull().WithCancellation(cts.Token))
			{
				result.Add(item!.Value);
				cts.Cancel();
			}
		}

		await That(Act).Throws<OperationCanceledException>().WithMessage("The operation was canceled.");
		await That(result).IsEqualTo([1,]);
	}

	[Test]
	public async Task Distinct_ShouldYieldEachItemOnlyOnce()
	{
		int[] source = [1, 1, 2, 3, 3, 3,];

		List<int> result = new();
		await foreach (int item in ToAsync(source).Distinct())
		{
			result.Add(item);
		}

		await That(result).IsEqualTo([1, 2, 3,]).InAnyOrder();
		await That(result).HasCount(3);
	}

	private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		foreach (T item in source)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await Task.Yield();
			yield return item;
		}
	}
#endif
}
