using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Core.Tests.Results;

public sealed class NullableNumberToleranceResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForNumberTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NullableNumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldSetTheTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NullableNumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		_ = sut.Within(1);

		await That(options.Value.Tolerance).IsEqualTo(1);
	}

	[Test]
	public async Task Within_WithRelativeTolerance_ShouldReplaceTheToleranceAndReturnTheResult()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NullableNumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);
		IWithinToleranceResult<NullableNumberToleranceResult<int, IThat<int>>, int> result = sut.Within(1);

		NullableNumberToleranceResult<int, IThat<int>> continuation =
			result.SetRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		await That(continuation).IsSameAs(result);
		await That(options.Value.ToleranceFor(200)).IsEqualTo(2);
		await That(options.Value.ToString()).IsEqualTo(" ± 1%");
	}

	private static NullableNumberToleranceResult<T, IThat<T>> CreateSut<T>(T subject,
		OptionsHolder<NumberTolerance<T>> options)
#if NET8_0_OR_GREATER
		where T : struct, INumber<T>
#else
		where T : struct, IComparable<T>
#endif
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new NullableNumberToleranceResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
