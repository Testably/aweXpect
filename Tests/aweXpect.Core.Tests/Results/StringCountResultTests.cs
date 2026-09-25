using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringCountResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForStringEqualityOptions()
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new("expected");
		StringCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier, options);

		await That(sut).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Fact]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new("expected");
		StringCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	private static StringCountResult<T, IThat<T>> CreateSut<T>(T subject, Quantifier quantifier,
		StringEqualityOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringCountResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			quantifier,
			options);
	}
}
