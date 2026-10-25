using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Core.Tests.Options;

public sealed class ToleranceExtensionsTests
{
	[Test]
	public async Task Percent_ShouldReturnTheResultOfWithin()
	{
		NumberTolerance<double> options = new((_, _) => null);
		NumberToleranceResult<double, IThat<double>>.WithinResult within = CreateSut(2.0, options).Within(5.0);

		NumberToleranceResult<double, IThat<double>> result = within.Percent();

		await That(result).IsSameAs(within);
	}

	[Test]
	public async Task Percent_WithDecimal_ShouldMakeTheToleranceRelative()
	{
		NumberTolerance<decimal> options = new((_, _) => null);

		_ = CreateSut(2m, options).Within(5m).Percent();

		await That(options.ToleranceFor(-200m)).IsEqualTo(10m);
		await That(options.ToString()).IsEqualTo(" ± 5.0%");
	}

	[Test]
	public async Task Percent_WithDecimal_WhenTheAbsoluteToleranceOverflows_ShouldCoverEveryDifference()
	{
		NumberTolerance<decimal> options = new((_, _) => null);

		_ = CreateSut(2m, options).Within(200m).Percent();

		await That(options.ToleranceFor(decimal.MaxValue)).IsEqualTo(decimal.MaxValue)
			.Because("200% of decimal.MaxValue is beyond the range of decimal");
	}

	[Test]
	public async Task Percent_WithDouble_ShouldMakeTheToleranceRelative()
	{
		NumberTolerance<double> options = new((_, _) => null);

		_ = CreateSut(2.0, options).Within(5.0).Percent();

		await That(options.ToleranceFor(-200.0)).IsEqualTo(10.0);
		await That(options.ToString()).IsEqualTo(" ± 5.0%");
	}

	[Test]
	public async Task Percent_WithFloat_ShouldMakeTheToleranceRelative()
	{
		NumberTolerance<float> options = new((_, _) => null);

		_ = CreateSut(2f, options).Within(5f).Percent();

		await That(options.ToleranceFor(-200f)).IsEqualTo(10f);
		await That(options.ToString()).IsEqualTo(" ± 5.0%");
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task Percent_WithHalf_ShouldMakeTheToleranceRelative()
	{
		NumberTolerance<Half> options = new((_, _) => null);

		_ = CreateSut((Half)2, options).Within((Half)5).Percent();

		await That(options.ToleranceFor((Half)(-200)) == (Half)10).IsTrue();
	}
#endif

	[Test]
	public async Task Within_WithByte_ShouldSetTheTolerance()
	{
		NumberTolerance<byte> options = new((_, _) => null);
		NumberToleranceResult<byte, IThat<byte>> sut = CreateSut((byte)2, options);

		NumberToleranceResult<byte, IThat<byte>> result = ToleranceExtensions.Within(sut,(byte)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((byte)1);
	}

	[Test]
	public async Task Within_WithInt_ShouldSetTheTolerance()
	{
		NumberTolerance<int> options = new((_, _) => null);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		NumberToleranceResult<int, IThat<int>> result = ToleranceExtensions.Within(sut,1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task Within_WithNuint_ShouldSetTheTolerance()
	{
		NumberTolerance<nuint> options = new((_, _) => null);
		NumberToleranceResult<nuint, IThat<nuint>> sut = CreateSut((nuint)2, options);

		NumberToleranceResult<nuint, IThat<nuint>> result = ToleranceExtensions.Within(sut,(nuint)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance == 1).IsTrue();
	}
#endif

	[Test]
	public async Task Within_WithSbyte_ShouldSetTheTolerance()
	{
		NumberTolerance<sbyte> options = new((_, _) => null);
		NumberToleranceResult<sbyte, IThat<sbyte>> sut = CreateSut((sbyte)2, options);

		NumberToleranceResult<sbyte, IThat<sbyte>> result = ToleranceExtensions.Within(sut,(sbyte)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((sbyte)1);
	}

	[Test]
	public async Task Within_WithShort_ShouldSetTheTolerance()
	{
		NumberTolerance<short> options = new((_, _) => null);
		NumberToleranceResult<short, IThat<short>> sut = CreateSut((short)2, options);

		NumberToleranceResult<short, IThat<short>> result = ToleranceExtensions.Within(sut,(short)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((short)1);
	}

	[Test]
	public async Task Within_WithUint_ShouldSetTheTolerance()
	{
		NumberTolerance<uint> options = new((_, _) => null);
		NumberToleranceResult<uint, IThat<uint>> sut = CreateSut(2u, options);

		NumberToleranceResult<uint, IThat<uint>> result = ToleranceExtensions.Within(sut,1u);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1u);
	}

	[Test]
	public async Task Within_WithUlong_ShouldSetTheTolerance()
	{
		NumberTolerance<ulong> options = new((_, _) => null);
		NumberToleranceResult<ulong, IThat<ulong>> sut = CreateSut(2ul, options);

		NumberToleranceResult<ulong, IThat<ulong>> result = ToleranceExtensions.Within(sut,1ul);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1ul);
	}

	[Test]
	public async Task Within_WithUshort_ShouldSetTheTolerance()
	{
		NumberTolerance<ushort> options = new((_, _) => null);
		NumberToleranceResult<ushort, IThat<ushort>> sut = CreateSut((ushort)2, options);

		NumberToleranceResult<ushort, IThat<ushort>> result = ToleranceExtensions.Within(sut,(ushort)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((ushort)1);
	}

	private static NumberToleranceResult<T, IThat<T>> CreateSut<T>(T subject, NumberTolerance<T> options)
#if NET8_0_OR_GREATER
		where T : struct, INumber<T>
#else
		where T : struct, IComparable<T>
#endif
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new NumberToleranceResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			new OptionsSlot<NumberTolerance<T>>(options));
	}
}
