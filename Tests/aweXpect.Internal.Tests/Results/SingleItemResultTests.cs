using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class SingleItemResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		PredicateOptions<int> options = new();
		SingleItemResult<string[], int> sut = CreateSut(Array.Empty<string>(), options, s => s.Length);

		await That(sut).Is<IOptionsProvider<PredicateOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static SingleItemResult<TCollection, TItem> CreateSut<TCollection, TItem>(TCollection subject,
		PredicateOptions<TItem> options,
		Func<TCollection, TItem?> memberAccessor)
	{
#pragma warning disable aweXpect0001
		IThat<TCollection> source = That(subject);
#pragma warning restore aweXpect0001
		return new SingleItemResult<TCollection, TItem>(source.Get().ExpectationBuilder,
			options, memberAccessor);
	}
}
