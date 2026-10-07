using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class HasItemWithConditionResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		PredicateOptions<int> options = new();
		HasItemWithConditionResult<int[], int> sut = CreateSut(Array.Empty<int>(), options);

		await That(sut).Is<IOptionsProvider<PredicateOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static HasItemWithConditionResult<TCollection, TItem> CreateSut<TCollection, TItem>(TCollection subject,
		PredicateOptions<TItem> options)
	{
#pragma warning disable aweXpect0001
		IThat<TCollection?> source = That(subject);
#pragma warning restore aweXpect0001
		return new HasItemWithConditionResult<TCollection, TItem>(source.Get().ExpectationBuilder,
			source, new CollectionIndexOptions(), options);
	}
}
