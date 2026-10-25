using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class ObjectEqualityWithToleranceResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForObjectMatchType()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ObjectMatchType<int>.WithTolerance<double>((_, _, _) => true));
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<ObjectMatchType<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldNotChangeTheStartingValue()
	{
		ObjectToleranceMatchType<int, double> start = ObjectMatchType<int>.WithTolerance<double>((_, _, _) => true);
		OptionsSlot<ObjectMatchType<int>> options = new(start);
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		_ = sut.Within(1.0);

		await That(options.Value).IsNotSameAs(start);
		await That(start.OptionName).IsNull();
		await That(start.ToString()).IsEmpty();
	}

	[Test]
	public async Task Within_ShouldSetTheTolerance()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ObjectMatchType<int>.WithTolerance<double>((_, _, _) => true));
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);

		_ = sut.Within(1.0);

		await That(options.ToString()).IsEqualTo(" ± 1.0");
	}

	[Test]
	public async Task Within_WithRelativeTolerance_ShouldMakeTheToleranceRelativeAndReturnTheResult()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ObjectMatchType<int>.WithTolerance<double>((_, _, _) => true));
		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> sut = CreateSut(2, options);
		IWithinToleranceResult<ObjectEqualityWithToleranceResult<int, IThat<int>, int, double>, double> result =
			sut.Within(1.0);

		ObjectEqualityWithToleranceResult<int, IThat<int>, int, double> continuation =
			result.WithRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		await That(continuation).IsSameAs(result);
		await That(options.ToString()).IsEqualTo(" ± 1.0%");
	}

	private static ObjectEqualityWithToleranceResult<int, IThat<int>, int, double>
		CreateSut(int subject, OptionsSlot<ObjectMatchType<int>> options)
	{
#pragma warning disable aweXpect0001
		IThat<int> source = That(subject);
#pragma warning restore aweXpect0001
		return new ObjectEqualityWithToleranceResult<int, IThat<int>, int, double>(
			source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
