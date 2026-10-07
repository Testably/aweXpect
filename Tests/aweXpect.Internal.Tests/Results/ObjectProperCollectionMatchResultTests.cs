using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class ObjectProperCollectionMatchResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForCollectionMatchOptions()
	{
		CollectionMatchOptions collectionMatchOptions = new();
		ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> sut = CreateSut([], new ObjectEqualityOptions<int>(),
			collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<CollectionMatchOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(collectionMatchOptions));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityOptions()
	{
		ObjectEqualityOptions<int> options = new();
		ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> sut = CreateSut([], options,
			new CollectionMatchOptions());

		await That(sut).Is<IOptionsProvider<ObjectEqualityOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> CreateSut(int[] subject,
		ObjectEqualityOptions<int> options, CollectionMatchOptions collectionMatchOptions)
	{
#pragma warning disable aweXpect0001
		IThat<int[]?> source = That(subject);
#pragma warning restore aweXpect0001
		return new ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int>(source.Get().ExpectationBuilder,
			source,
			options,
			collectionMatchOptions);
	}
}
