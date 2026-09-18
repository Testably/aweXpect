using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class AsyncSingleItemResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		PredicateOptions<int> options = new();
		AsyncSingleItemResult<string[], int> sut = CreateSut(Array.Empty<string>(), options,
			s => Task.FromResult(s.Length));

		await That(sut).Is<IOptionsProvider<PredicateOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static AsyncSingleItemResult<TCollection, TItem> CreateSut<TCollection, TItem>(TCollection subject,
		PredicateOptions<TItem> options,
		Func<TCollection, Task<TItem?>> memberAccessor)
	{
#pragma warning disable aweXpect0001
		IThat<TCollection> source = That(subject);
#pragma warning restore aweXpect0001
		return new AsyncSingleItemResult<TCollection, TItem>(source.Get().ExpectationBuilder,
			options, memberAccessor);
	}
}
