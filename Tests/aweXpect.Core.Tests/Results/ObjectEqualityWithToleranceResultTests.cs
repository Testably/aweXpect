using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
#endif

namespace aweXpect.Core.Tests.Results;

public sealed class ObjectEqualityWithToleranceResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityOptions()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<ObjectEqualityOptions<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectEqualityWithToleranceOptions()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<ObjectEqualityWithToleranceOptions<int, double>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldSetTheTolerance()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		_ = sut.Within(1.0);

		await That(options.ToString()).IsEqualTo(" ± 1.0");
	}

	[Test]
	public async Task Within_WithRelativeTolerance_ShouldMakeTheToleranceRelativeAndReturnTheResult()
	{
		ObjectEqualityWithToleranceOptions<int, double> options = new((_, _, _) => true);
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);
		IWithinToleranceResult<ObjectEqualityWithToleranceResult<int, IThat<int>, int, double>, double> result =
			sut.Within(1.0);

		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> continuation =
			result.WithRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		await That(continuation).IsSameAs(result);
		await That(options.ToString()).IsEqualTo(" ± 1.0%");
	}

	private static ObjectEqualityWithToleranceResult<TType, IThat<TType>, TElement, TTolerance>
		CreateSut<TType, TElement, TTolerance>(TType subject,
			ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	{
#pragma warning disable aweXpect0001
		IThat<TType> source = That(subject);
#pragma warning restore aweXpect0001
		return new ObjectEqualityWithToleranceResult<TType, IThat<TType>, TElement, TTolerance>(
			source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			new OptionsSlot<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>(options));
	}
}
