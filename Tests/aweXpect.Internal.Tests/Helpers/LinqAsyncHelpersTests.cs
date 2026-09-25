using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class LinqAsyncHelpersTests
{
	public sealed class AnyButNotInDuplicatesAsyncTests
	{
		[Fact]
		public async Task WhenMissingInSource_ShouldBeFalse()
		{
			int[] source = [1, 2, 3,];
			int[] duplicates = [2, 3, 42,];

			bool result = await source.AnyButNotInDuplicatesAsync(duplicates, Is42Predicate);

			await That(result).IsFalse();
		}

		[Fact]
		public async Task WhenOccursInSourceAndInDuplicates_ShouldBeFalse()
		{
			int[] source = [1, 2, 3, 42,];
			int[] duplicates = [2, 3, 42,];

			bool result = await source.AnyButNotInDuplicatesAsync(duplicates, Is42Predicate);

			await That(result).IsFalse();
		}

		[Fact]
		public async Task WhenOccursInSourceButNotInDuplicates_ShouldBeTrue()
		{
			int[] source = [1, 2, 3, 42,];
			int[] duplicates = [2, 3,];

			bool result = await source.AnyButNotInDuplicatesAsync(duplicates, Is42Predicate);

			await That(result).IsTrue();
		}
	}

	private static ValueTask<bool> Is42Predicate(int value) => new ValueTask<bool>(value == 42);
}
