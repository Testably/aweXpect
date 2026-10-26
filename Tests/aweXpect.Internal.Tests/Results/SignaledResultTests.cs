using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Internal.Tests.Results;

public sealed class SignaledResultTests
{
	[Test]
	public async Task Generic_ShouldBeOptionsProvider_ForPredicateOptions()
	{
		OptionsHolder<SignalerPredicateOptions<int>> predicates = new(SignalerPredicateOptions<int>.Default);
		SignaledWhoseResult<int> sut = CreateSut(new OptionsHolder<SignalerOptions>(SignalerOptions.Default), predicates);

		await That(sut).Is<IOptionsProvider<SignalerPredicateOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(predicates));
	}

	[Test]
	public async Task Generic_ShouldBeOptionsProvider_ForSignalerOptions()
	{
		OptionsHolder<SignalerOptions> options = new(SignalerOptions.Default);
		SignaledWhoseResult<int> sut =
			CreateSut(options, new OptionsHolder<SignalerPredicateOptions<int>>(SignalerPredicateOptions<int>.Default));

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Generic_ShouldNotBeOptionsProvider_ForQuantifier()
	{
		SignaledWhoseResult<int> sut = CreateSut(new OptionsHolder<SignalerOptions>(SignalerOptions.Default),
			new OptionsHolder<SignalerPredicateOptions<int>>(SignalerPredicateOptions<int>.Default));

		await That(sut).IsNot<IOptionsProvider<Quantifier>>()
			.Because("the number of times is already given, so a quantifier would silently replace it");
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		OptionsHolder<SignalerOptions> options = new(SignalerOptions.Default);
		SignaledResult sut = CreateSut(options);

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldNotBeOptionsProvider_ForQuantifier()
	{
		SignaledResult sut = CreateSut(new OptionsHolder<SignalerOptions>(SignalerOptions.Default));

		await That(sut).IsNot<IOptionsProvider<Quantifier>>()
			.Because("the number of times is already given, so a quantifier would silently replace it");
	}

	private static SignaledWhoseResult<TParameter> CreateSut<TParameter>(OptionsHolder<SignalerOptions> options,
		OptionsHolder<SignalerPredicateOptions<TParameter>> predicates)
	{
		Signaler<TParameter> signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler<TParameter>> source = That(signaler);
#pragma warning restore aweXpect0001
		return new SignaledWhoseResult<TParameter>(source.Get().ExpectationBuilder,
			source, options, predicates);
	}

	private static SignaledResult CreateSut(OptionsHolder<SignalerOptions> options)
	{
		Signaler signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler> source = That(signaler);
#pragma warning restore aweXpect0001
		return new SignaledResult(source.Get().ExpectationBuilder,
			source, options);
	}
}
