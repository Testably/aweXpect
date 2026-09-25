using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class StringCollectionMatchResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForCollectionMatchOptions()
	{
		StringEqualityOptions options = new("expected");
		CollectionMatchOptions collectionMatchOptions = new();
		StringCollectionMatchResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<CollectionMatchOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(collectionMatchOptions));
	}

	[Fact]
	public async Task ShouldBeOptionsProvider_ForStringEqualityOptions()
	{
		StringEqualityOptions options = new("expected");
		CollectionMatchOptions collectionMatchOptions = new();
		StringCollectionMatchResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static StringCollectionMatchResult<T, IThat<T>> CreateSut<T>(T subject,
		StringEqualityOptions options, CollectionMatchOptions collectionMatchOptions)
		where T : notnull
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringCollectionMatchResult<T, IThat<T>>(source.Get().ExpectationBuilder,
			source,
			options, collectionMatchOptions);
	}
}
