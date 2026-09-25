using System.Collections.Generic;
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

	[Fact]
	public async Task SetMatchType_WithOptionName_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		void Act() => sut.SetMatchType(new DummyMatchType(), "Custom");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Custom cannot be combined with Using.")
			.Because("the match type would silently replace the comparer");
	}

	[Fact]
	public async Task SetMatchType_WithOptionName_WhenTheSameOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.SetMatchType(new DummyMatchType(), "Custom");

		void Act() => sut.SetMatchType(new DummyMatchType(), "Custom");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Custom cannot be specified more than once.")
			.Because("the second match type would silently replace the first one");
	}

	[Fact]
	public async Task SetMatchType_WithoutOptionName_ShouldReplaceTheMatchType()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		sut.SetMatchType(new DummyMatchType());

		await That(sut.ToString()).IsEqualTo("dummy")
			.Because("without an option name the match type is set as is, e.g. to resolve a default");
	}

	[Fact]
	public async Task Using_WhenAComparerIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be specified more than once.")
			.Because("the second comparer would silently replace the first one");
	}

	[Fact]
	public async Task Using_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.SetMatchType(new DummyMatchType(), "Custom");

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Custom.")
			.Because("the comparer would silently replace the match type");
	}

	[Fact]
	public async Task Using_WithNull_ShouldThrowArgumentNullException()
	{
		ObjectEqualityOptions<object> sut = new();

		void Act() => sut.Using(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
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

	private sealed class AllEqualComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => true;

		public int GetHashCode(object obj) => 0;
	}

	private sealed class DummyMatchType : IObjectMatchType
	{
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(true);

		public string GetExpectation(string expected, ExpectationGrammars grammars) => "";

		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> "";

		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> "";

		public override string ToString() => "dummy";
	}
}
