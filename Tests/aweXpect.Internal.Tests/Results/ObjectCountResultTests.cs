using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class ObjectCountResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectMatchType()
	{
		OptionsHolder<ObjectMatchType<string>> options = new(ObjectMatchType<string>.Default);
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		ObjectCountResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<ObjectMatchType<string>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		OptionsHolder<ObjectMatchType<string>> options = new(ObjectMatchType<string>.Default);
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		ObjectCountResult<ObjectCollectionMatchResultTests, IThat<ObjectCollectionMatchResultTests>, string>
			sut = CreateSut(new ObjectCollectionMatchResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	private static ObjectCountResult<T, IThat<T>, TElement> CreateSut<T, TElement>(T subject, OptionsHolder<Quantifier> quantifier,
		OptionsHolder<ObjectMatchType<TElement>> options)
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
