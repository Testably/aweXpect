using System.Collections.Generic;
using System.Linq;
#if NET8_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class LinqAsyncHelpersTests
{
	[Fact]
	public async Task Split_ShouldSeparateMatchingFromNotMatching()
	{
		int[] source = [1, 2, 3, 4,];

		(int[] matching, int[] notMatching) = source.Split(i => i % 2 == 0);

		await That(matching).IsEqualTo([2, 4,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1, 3,]).InAnyOrder();
	}

	[Fact]
	public async Task SplitAsync_ShouldSeparateMatchingFromNotMatching()
	{
		int[] source = [1, 2, 3, 4,];

#if NET8_0_OR_GREATER
		(int[] matching, int[] notMatching) =
			await source.SplitAsync(i => new ValueTask<bool>(i % 2 == 0));
#else
		(int[] matching, int[] notMatching) =
			await source.SplitAsync(i => Task.FromResult(i % 2 == 0));
#endif

		await That(matching).IsEqualTo([2, 4,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1, 3,]).InAnyOrder();
	}

	[Fact]
	public async Task SplitWhereAnyAsync_ShouldSeparateBasedOnGeneratedItems()
	{
		int[] source = [1, 2, 3,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			i => Enumerable.Range(0, i),
#if NET8_0_OR_GREATER
			target => new ValueTask<bool>(target > 0));
#else
			target => Task.FromResult(target > 0));
#endif

		// generator(1) => [0]            => no value > 0 => not matching
		// generator(2) => [0, 1]         => contains 1   => matching
		// generator(3) => [0, 1, 2]      => contains 1,2 => matching
		await That(matching).IsEqualTo([2, 3,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1,]).InAnyOrder();
	}

	[Fact]
	public async Task SplitWhereAnyAsync_WhenGeneratorReturnsNull_ShouldNotMatch()
	{
		int[] source = [1, 2,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			i => i == 1 ? null : Enumerable.Range(0, 5),
#if NET8_0_OR_GREATER
			target => new ValueTask<bool>(target >= 0));
#else
			target => Task.FromResult(target >= 0));
#endif

		// generator(1) => null => not matching (must not call AnyAsync on null)
		// generator(2) => [0..4] => contains values >= 0 => matching
		await That(matching).IsEqualTo([2,]).InAnyOrder();
		await That(notMatching).IsEqualTo([1,]).InAnyOrder();
	}

	[Fact]
	public async Task SplitWhereAnyAsync_WhenGeneratorYieldsNoMatch_ShouldNotMatch()
	{
		int[] source = [1, 2,];

		(int[] matching, int[] notMatching) = await source.SplitWhereAnyAsync(
			_ => Enumerable.Range(0, 3),
#if NET8_0_OR_GREATER
			target => new ValueTask<bool>(target > 100));
#else
			target => Task.FromResult(target > 100));
#endif

		// No generated value is > 100 => nothing matches.
		await That(matching).IsEmpty();
		await That(notMatching).IsEqualTo([1, 2,]).InAnyOrder();
	}

#if NET8_0_OR_GREATER
	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task Distinct_ShouldYieldEachItemOnlyOnce()
	{
		int[] source = [1, 1, 2, 3, 3, 3,];

		List<int> result = new();
		await foreach (int item in LinqAsyncHelpers.Distinct(ToAsync(source)))
		{
			result.Add(item);
		}

		await That(result).IsEqualTo([1, 2, 3,]).InAnyOrder();
		await That(result).HasCount(3);
	}

	private static async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
	{
		foreach (T item in source)
		{
			await Task.Yield();
			yield return item;
		}
	}
#endif
}
