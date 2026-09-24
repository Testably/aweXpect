using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ObjectEqualityOptionsTests
{
	[Theory]
	[MemberData(nameof(DifferentNumbers), DisableDiscoveryEnumeration = true)]
	public async Task AreConsideredEqual_WhenNumbersHaveDifferentValues_ShouldReturnFalse(
		object actual, object expected)
	{
		ObjectEqualityOptions<object> sut = new();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsFalse()
			.Because("a value that does not fit into the other type must neither wrap around nor lose precision to an equal value");
	}

	[Theory]
	[MemberData(nameof(EqualNumbers), DisableDiscoveryEnumeration = true)]
	public async Task AreConsideredEqual_WhenNumbersHaveSameValue_ShouldReturnTrue(
		object actual, object expected)
	{
		ObjectEqualityOptions<object> sut = new();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsTrue();
	}

	public static TheoryData<object, object> DifferentNumbers() => new()
	{
		{
			-1, uint.MaxValue
		},
		{
			(sbyte)-1, (byte)255
		},
		{
			int.MinValue, 2147483648u
		},
		{
			-1L, ulong.MaxValue
		},
		{
			int.MaxValue, 2147483648f
		},
		{
			1.5, 1
		},
		{
			double.NaN, 0
		},
		{
			double.PositiveInfinity, long.MaxValue
		},
#if NET8_0_OR_GREATER
		{
			(Int128)(-1), UInt128.MaxValue
		},
		{
			(nint)(-1), nuint.MaxValue
		},
		{
			(Half)(-1), ushort.MaxValue
		},
		{
			Half.PositiveInfinity, 65536
		},
#endif
	};

	public static TheoryData<object, object> EqualNumbers() => new()
	{
		{
			1, 1L
		},
		{
			1.0, 1
		},
		{
			-1, (sbyte)-1
		},
		{
			(decimal)6, 6
		},
		{
			uint.MaxValue, (long)uint.MaxValue
		},
#if NET8_0_OR_GREATER
		{
			(nint)1, 1
		},
		{
			(Half)10, 10
		},
		{
			(Int128)(-1), -1L
		},
		{
			(UInt128)ulong.MaxValue, ulong.MaxValue
		},
#endif
	};
}
