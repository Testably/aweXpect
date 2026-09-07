using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringBlockCountResultTests
{
	[Fact]
	public async Task AsBlock_ShouldInterpretExpectedAsBlock()
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		StringEqualityTypeCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), quantifier, options);

		StringBlockCountResult<int[], IThat<int[]>> result = sut.AsBlock();

		await That(options.ToString()).IsEqualTo(" as block");
		await That(result).Is<IOptionsProvider<StringEqualityOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
		await That(result).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	[Fact]
	public async Task IgnoringCase_ShouldSetOption()
	{
		StringEqualityOptions options = new();
		StringBlockCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), new Quantifier(), options)
			.AsBlock();

		StringBlockCountResult<int[], IThat<int[]>> result = sut.IgnoringCase();

		await That(result).IsSameAs(sut);
		await That(options.ToString()).IsEqualTo(" as block ignoring case");
	}

	[Fact]
	public async Task Using_ShouldSetComparer()
	{
		StringEqualityOptions options = new();
		StringBlockCountResult<int[], IThat<int[]>> sut = CreateSut(Array.Empty<int>(), new Quantifier(), options)
			.AsBlock();

		StringBlockCountResult<int[], IThat<int[]>> result = sut.Using(StringComparer.OrdinalIgnoreCase);

		await That(result).IsSameAs(sut);
		await That(options.ToString()).StartsWith(" as block using ");
	}

	private static StringEqualityTypeCountResult<T, IThat<T>> CreateSut<T>(T subject, Quantifier quantifier,
		StringEqualityOptions options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new StringEqualityTypeCountResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			quantifier,
			options);
	}
}
