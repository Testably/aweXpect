using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Internal.Tests.Results;

public sealed class SignaledResultTests
{
	[Test]
	public async Task Generic_ShouldBeOptionsProvider_ForPredicateOptions()
	{
		SignalerOptions<int> options = new();
		SignaledWhoseResult<int> sut = CreateSut(options);

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Generic_ShouldNotBeOptionsProvider_ForQuantifier()
	{
		SignaledWhoseResult<int> sut = CreateSut(new SignalerOptions<int>());

		await That(sut).IsNot<IOptionsProvider<Quantifier>>()
			.Because("the number of times is already given, so a quantifier would silently replace it");
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		SignalerOptions options = new();
		SignaledResult sut = CreateSut(options);

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldNotBeOptionsProvider_ForQuantifier()
	{
		SignaledResult sut = CreateSut(new SignalerOptions());

		await That(sut).IsNot<IOptionsProvider<Quantifier>>()
			.Because("the number of times is already given, so a quantifier would silently replace it");
	}

	private static SignaledWhoseResult<TParameter> CreateSut<TParameter>(SignalerOptions<TParameter> options)
	{
		Signaler<TParameter> signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler<TParameter>> source = That(signaler);
#pragma warning restore aweXpect0001
		return new SignaledWhoseResult<TParameter>(source.Get().ExpectationBuilder,
			source, options);
	}

	private static SignaledResult CreateSut(SignalerOptions options)
	{
		Signaler signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler> source = That(signaler);
#pragma warning restore aweXpect0001
		return new SignaledResult(source.Get().ExpectationBuilder,
			source, options);
	}
}
