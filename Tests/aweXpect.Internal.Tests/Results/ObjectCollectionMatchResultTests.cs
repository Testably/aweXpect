using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class ObjectCollectionMatchResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForCollectionMatchOptions()
	{
		ObjectEqualityOptions<string> options = new();
		CollectionMatchOptions collectionMatchOptions = new();
		ObjectCollectionMatchResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<CollectionMatchOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(collectionMatchOptions));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityOptions()
	{
		ObjectEqualityOptions<string> options = new();
		CollectionMatchOptions collectionMatchOptions = new();
		ObjectCollectionMatchResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<ObjectEqualityOptions<string>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ObjectCollectionMatchResult<T, IThat<T>, TElement> CreateSut<T, TElement>(T subject,
		ObjectEqualityOptions<TElement> options, CollectionMatchOptions collectionMatchOptions)
		where T : notnull
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new ObjectCollectionMatchResult<T, IThat<T>, TElement>(source.Get().ExpectationBuilder,
			source,
			options, collectionMatchOptions);
	}
}
