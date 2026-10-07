using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Internal.Tests.Results;

public sealed class SignalCountResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForPredicateOptions()
	{
		SignalerOptions options = new();
		SignalCountResult sut = CreateSut(options);

		await That(sut).Is<IOptionsProvider<SignalerOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static SignalCountResult CreateSut(SignalerOptions options)
	{
		Signaler signaler = new();
#pragma warning disable aweXpect0001
		IThat<Signaler> source = That(signaler);
#pragma warning restore aweXpect0001
		return new SignalCountResult(source.Get().ExpectationBuilder,
			source, new Quantifier(), options);
	}
}
