using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringOccurrenceCountResultTests
{
	[Test]
	public async Task AsBlock_ShouldInterpretExpectedAsBlock()
	{
		OptionsSlot<Quantifier> quantifier = new(Quantifier.Default);
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		StringOccurrenceCountResult<string, IThat<string>> sut = CreateSut("foo", quantifier, options);

		StringBlockCountResult<string, IThat<string>> result = sut.AsBlock();

		await That(options.ToString()).IsEqualTo(" as block");
		await That(result).IsNot<IOptionsProvider<StringEqualityOptions>>()
			.Because("a block compares the lines on its own, so it offers only the casing and a comparer");
		await That(result).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	[Test]
	public async Task ShouldNotOfferPrefixAndSuffixMatchTypes()
	{
		StringOccurrenceCountResult<string, IThat<string>> sut =
			CreateSut("foo", new OptionsSlot<Quantifier>(Quantifier.Default), new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));

		await That(sut).IsNot<IStringMatchTypeOptions>()
			.Because("an occurrence can be anywhere in the string, so AsPrefix() and AsSuffix() must not compile");
	}

	[Test]
	public async Task ShouldOfferPatternMatchTypes()
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		StringOccurrenceCountResult<string, IThat<string>> sut = CreateSut("foo", new OptionsSlot<Quantifier>(Quantifier.Default), options);

		StringOccurrenceCountResult<string, IThat<string>> result = sut.AsWildcard();

		await That(result).IsSameAs(sut);
		await That(sut).Is<IStringPatternMatchTypeOptions>();
		await That(options.ToString()).IsEqualTo(" as wildcard");
	}

	[Test]
	public async Task ShouldProvideTheQuantifierAndTheStringOptions()
	{
		OptionsSlot<Quantifier> quantifier = new(Quantifier.Default);
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		StringOccurrenceCountResult<string, IThat<string>> sut = CreateSut("foo", quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
		await That(sut).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static StringOccurrenceCountResult<T, IThat<T>> CreateSut<T>(T subject, OptionsSlot<Quantifier> quantifier,
		OptionsSlot<StringEqualityOptions> options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringOccurrenceCountResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			quantifier,
			options);
	}
}
