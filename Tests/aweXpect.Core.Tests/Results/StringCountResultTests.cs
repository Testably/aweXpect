using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringCountResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		OptionsSlot<Quantifier> quantifier = new(Quantifier.Default);
		StringEqualityOptions options = StringEqualityOptions.Default;
		StringCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForStringEqualityOptions()
	{
		OptionsSlot<Quantifier> quantifier = new(Quantifier.Default);
		StringEqualityOptions options = StringEqualityOptions.Default;
		StringCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier, options);

		await That(sut).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	private static StringCountResult<T, IThat<T>> CreateSut<T>(T subject, OptionsSlot<Quantifier> quantifier,
		StringEqualityOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringCountResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			quantifier,
			new OptionsSlot<StringEqualityOptions>(options));
	}
}
