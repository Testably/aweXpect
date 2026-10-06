using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
#endif

namespace aweXpect.Core.Tests.Results;

public sealed class ToleranceEqualityResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityOptions()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ToleranceEqualityResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<ObjectEqualityOptions<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityWithToleranceOptions()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ToleranceEqualityResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<ObjectEqualityWithToleranceOptions<int, double>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ToleranceEqualityResult<TType, IThat<TType>, TElement, TTolerance>
		CreateSut<TType, TElement, TTolerance>(TType subject,
			ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	{
#pragma warning disable aweXpect0001
		IThat<TType> source = That(subject);
#pragma warning restore aweXpect0001
		return new ToleranceEqualityResult<TType, IThat<TType>, TElement, TTolerance>(
			source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
