using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class CollectionOrderResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForCollectionOrderOptions()
	{
		CollectionOrderOptions<int> options = new();
		CollectionOrderResult<int, int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), options);

		await That(sut).Is<IOptionsProvider<CollectionOrderOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static CollectionOrderResult<TMember, T, IThat<T>> CreateSut<T, TMember>(T subject,
		CollectionOrderOptions<TMember> options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new CollectionOrderResult<TMember, T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
