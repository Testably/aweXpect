using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class HasItemAtIndexResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForCollectionIndexOptions()
	{
		CollectionIndexOptions options = new();
		HasItemAtIndexResult<int[]> sut = CreateSut(Array.Empty<int>(), options);

		await That(sut).Is<IOptionsProvider<CollectionIndexOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static HasItemAtIndexResult<T> CreateSut<T>(T subject, CollectionIndexOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new HasItemAtIndexResult<T>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
