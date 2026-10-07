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
	public async Task Within_WithByte_ShouldSetTheTolerance()
	{
		NumberTolerance<byte> options = new((_, _) => null);
		NumberToleranceResult<byte, IThat<byte>> sut = CreateSut((byte)2, options);

		NumberToleranceResult<byte, IThat<byte>> result = sut.Within((byte)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((byte)1);
	}

	[Test]
	public async Task Within_WithInt_ShouldSetTheTolerance()
	{
		NumberTolerance<int> options = new((_, _) => null);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		NumberToleranceResult<int, IThat<int>> result = sut.Within(1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task Within_WithNuint_ShouldSetTheTolerance()
	{
		NumberTolerance<nuint> options = new((_, _) => null);
		NumberToleranceResult<nuint, IThat<nuint>> sut = CreateSut((nuint)2, options);

		NumberToleranceResult<nuint, IThat<nuint>> result = sut.Within((nuint)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance == 1).IsTrue();
	}
#endif

	[Test]
	public async Task Within_WithSbyte_ShouldSetTheTolerance()
	{
		NumberTolerance<sbyte> options = new((_, _) => null);
		NumberToleranceResult<sbyte, IThat<sbyte>> sut = CreateSut((sbyte)2, options);

		NumberToleranceResult<sbyte, IThat<sbyte>> result = sut.Within((sbyte)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((sbyte)1);
	}

	[Test]
	public async Task Within_WithShort_ShouldSetTheTolerance()
	{
		NumberTolerance<short> options = new((_, _) => null);
		NumberToleranceResult<short, IThat<short>> sut = CreateSut((short)2, options);

		NumberToleranceResult<short, IThat<short>> result = sut.Within((short)1);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo((short)1);
	}

	[Test]
	public async Task Within_WithUint_ShouldSetTheTolerance()
	{
		NumberTolerance<uint> options = new((_, _) => null);
		NumberToleranceResult<uint, IThat<uint>> sut = CreateSut(2u, options);

		NumberToleranceResult<uint, IThat<uint>> result = sut.Within(1u);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1u);
	}

	[Test]
	public async Task Within_WithUlong_ShouldSetTheTolerance()
	{
		NumberTolerance<ulong> options = new((_, _) => null);
		NumberToleranceResult<ulong, IThat<ulong>> sut = CreateSut(2ul, options);

		NumberToleranceResult<ulong, IThat<ulong>> result = sut.Within(1ul);

		await That(result).IsSameAs(sut);
		await That(options.Tolerance).IsEqualTo(1ul);
	}

	[Test]
	public async Task Within_WithUshort_ShouldSetTheTolerance()
	{
		NumberTolerance<ushort> options = new((_, _) => null);
		NumberToleranceResult<ushort, IThat<ushort>> sut = CreateSut((ushort)2, options);

		NumberToleranceResult<ushort, IThat<ushort>> result = sut.Within((ushort)1);

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
			options);
	}
}
