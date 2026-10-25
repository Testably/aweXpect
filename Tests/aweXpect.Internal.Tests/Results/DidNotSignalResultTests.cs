using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Internal.Tests.Results;

public sealed class DidNotSignalResultTests
{
	[Test]
	public async Task Generic_ShouldBeOptionsProvider_ForPredicateOptions()
	{
		OptionsSlot<SignalerPredicateOptions<int>> predicates = new(SignalerPredicateOptions<int>.Default);
		DidNotSignalResult<int> sut = CreateSut(new OptionsSlot<SignalerOptions>(SignalerOptions.Default), predicates);

		await That(sut).Is<IOptionsProvider<SignalerPredicateOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(predicates));
	}

	[Test]
	public async Task Generic_ShouldBeOptionsProvider_ForSignalerOptions()
	{
		OptionsSlot<SignalerOptions> options = new(SignalerOptions.Default);
		DidNotSignalResult<int> sut =
			CreateSut(options, new OptionsSlot<SignalerPredicateOptions<int>>(SignalerPredicateOptions<int>.Default));

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		OptionsSlot<SignalerOptions> options = new(SignalerOptions.Default);
		DidNotSignalResult sut = CreateSut(options);

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static DidNotSignalResult<TParameter> CreateSut<TParameter>(OptionsSlot<SignalerOptions> options,
		OptionsSlot<SignalerPredicateOptions<TParameter>> predicates)
	{
		Signaler<TParameter> signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler<TParameter>> source = That(signaler);
#pragma warning restore aweXpect0001
		return new DidNotSignalResult<TParameter>(source.Get().ExpectationBuilder,
			source, options, predicates);
	}

	private static DidNotSignalResult CreateSut(OptionsSlot<SignalerOptions> options)
	{
		Signaler signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler> source = That(signaler);
#pragma warning restore aweXpect0001
		return new DidNotSignalResult(source.Get().ExpectationBuilder,
			source, options);
	}
}
