using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class ObjectProperCollectionMatchResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForCollectionMatchOptions()
	{
		OptionsSlot<CollectionMatchOptions> collectionMatchOptions = new(new CollectionMatchOptions());
		ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> sut = CreateSut([], new OptionsSlot<ObjectMatchType<int>>(ObjectMatchType<int>.Default),
			collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<CollectionMatchOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(collectionMatchOptions));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectMatchType()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ObjectMatchType<int>.Default);
		ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> sut = CreateSut([], options,
			new OptionsSlot<CollectionMatchOptions>(new CollectionMatchOptions()));

		await That(sut).Is<IOptionsProvider<ObjectMatchType<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ObjectProperCollectionMatchResult<int[], IThat<int[]?>, int> CreateSut(int[] subject,
		OptionsSlot<ObjectMatchType<int>> options, OptionsSlot<CollectionMatchOptions> collectionMatchOptions)
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
