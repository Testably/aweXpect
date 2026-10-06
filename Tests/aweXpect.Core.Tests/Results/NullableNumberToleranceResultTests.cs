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
		NumberTolerance<int> options = new((a, b) => a - b);
		NullableNumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static NullableNumberToleranceResult<T, IThat<T>> CreateSut<T>(T subject, NumberTolerance<T> options)
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
