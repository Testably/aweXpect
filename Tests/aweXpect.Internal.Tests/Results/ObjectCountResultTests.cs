using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class ObjectCountResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityOptions()
	{
		ObjectEqualityOptions<string> options = new();
		Quantifier quantifier = new();
		ObjectCountResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<ObjectEqualityOptions<string>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		ObjectEqualityOptions<string> options = new();
		Quantifier quantifier = new();
		ObjectCountResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	private static ObjectCountResult<T, IThat<T>, TElement> CreateSut<T, TElement>(T subject, Quantifier quantifier,
		ObjectEqualityOptions<TElement> options)
		where T : notnull
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new ObjectCountResult<T, IThat<T>, TElement>(source.Get().ExpectationBuilder,
			source,
			quantifier,
			options);
	}
}
