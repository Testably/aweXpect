using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class CountResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		Quantifier quantifier = new();
		CountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	private static CountResult<T, IThat<T>> CreateSut<T>(T subject, Quantifier quantifier)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new CountResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			quantifier);
	}
}
