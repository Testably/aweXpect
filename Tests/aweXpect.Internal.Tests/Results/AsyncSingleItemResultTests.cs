using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class AsyncSingleItemResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		PredicateOptions<int> options = new();
		AsyncSingleItemResult<string[], int> sut = CreateSut(Array.Empty<string>(), options);

		await That(sut).Is<IOptionsProvider<PredicateOptions<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	private static AsyncSingleItemResult<TCollection, TItem> CreateSut<TCollection, TItem>(TCollection subject,
		PredicateOptions<TItem> options)
	{
#pragma warning disable aweXpect0001
		IThat<TCollection> source = That(subject);
#pragma warning restore aweXpect0001
		return new AsyncSingleItemResult<TCollection, TItem>(source.Get().ExpectationBuilder,
			new OptionsSlot<PredicateOptions<TItem>>(options), new SingleItemSource<TItem>());
	}
}
