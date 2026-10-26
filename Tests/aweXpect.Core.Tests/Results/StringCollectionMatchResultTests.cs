using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringCollectionMatchResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForCollectionMatchOptions()
	{
		StringEqualityOptions options = StringEqualityOptions.Default;
		OptionsHolder<CollectionMatchOptions> collectionMatchOptions = new(new CollectionMatchOptions());
		StringCollectionMatchResult<string[], IThat<string[]>> sut =
			CreateSut(Array.Empty<string>(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<CollectionMatchOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(collectionMatchOptions));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForStringEqualityOptions()
	{
		StringEqualityOptions options = StringEqualityOptions.Default;
		OptionsHolder<CollectionMatchOptions> collectionMatchOptions = new(new CollectionMatchOptions());
		StringCollectionMatchResult<string[], IThat<string[]>> sut =
			CreateSut(Array.Empty<string>(), options, collectionMatchOptions);

		await That(sut).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	private static StringCollectionMatchResult<T, IThat<T>> CreateSut<T>(T subject,
		StringEqualityOptions options, OptionsHolder<CollectionMatchOptions> collectionMatchOptions)
		where T : notnull
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringCollectionMatchResult<T, IThat<T>>(source.Get().ExpectationBuilder,
			source,
			new OptionsHolder<StringEqualityOptions>(options), collectionMatchOptions);
	}
}
