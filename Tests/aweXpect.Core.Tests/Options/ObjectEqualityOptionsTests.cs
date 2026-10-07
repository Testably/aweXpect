using System.Collections.Generic;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ObjectEqualityOptionsTests
{
	[Test]
	public async Task AreConsideredEqualWithExplanation_WhenEqual_ShouldReturnAMatch()
	{
		ObjectEqualityOptions<int> sut = new();

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(1, 1);

		await That(result.IsMatch).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqualWithExplanation_WhenNotEqual_ShouldExplainWithTheActualValue()
	{
		ObjectEqualityOptions<int> sut = new();

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(1, 2);

		await That(result.IsMatch).IsFalse();
		await That(result.GetExtendedFailure("it", ExpectationGrammars.None, 1, 2)).IsEqualTo("it was 1");
	}

	[Test]
	[Arguments(11, true)]
	[Arguments(12, false)]
	public async Task AreConsideredEqualWithExplanation_WithATypedComparer_ShouldDecideWithIt(int expected,
		bool expectMatch)
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(1, expected);

		await That(result.IsMatch).IsEqualTo(expectMatch);
		await That(result.GetExtendedFailure("it", ExpectationGrammars.None, 1, expected)).IsEqualTo("it was 1");
	}

	[Test]
	public async Task AreConsideredEqualWithExplanation_WithAnUntypedComparer_ShouldDecideWithIt()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		IObjectMatchResult result = await sut.AreConsideredEqualWithExplanation(1, "foo");

		await That(result.IsMatch).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqual_WhenBothAreNull_ShouldReturnTrue()
	{
		ObjectEqualityOptions<string?> sut = new();

		bool result = await sut.AreConsideredEqual(null, (string?)null);

		await That(result).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqual_WhenEqualsThrows_ShouldNameTheEqualsOfTheSubject()
	{
		InvalidOperationException exception = new("equals failed");
		ObjectEqualityOptions<ThrowingEquals> sut = new();

		async Task Act()
			=> await sut.AreConsideredEqual(new ThrowingEquals(exception), new ThrowingEquals(exception));

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("Equals of ObjectEqualityOptionsTests.ThrowingEquals")).And
			.Whose(e => e.Exception, inner => inner.IsSameAs(exception));
	}

	[Test]
	public async Task AreConsideredEqual_WhenEqualsThrowsForAnotherType_ShouldNameTheEqualsOfTheSubject()
	{
		InvalidOperationException exception = new("equals failed");
		ObjectEqualityOptions<ThrowingEquals> sut = new();

		async Task Act()
			=> await sut.AreConsideredEqual(new ThrowingEquals(exception), "foo");

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("Equals of ObjectEqualityOptionsTests.ThrowingEquals")).And
			.Whose(e => e.Exception, inner => inner.IsSameAs(exception))
			.Because("a value of another type is compared with Equals(object) of the subject");
	}

	[Test]
	[MethodDataSource(nameof(DifferentNumbers))]
	public async Task AreConsideredEqual_WhenNumbersHaveDifferentValues_ShouldReturnFalse(
		object actual, object expected)
	{
		ObjectEqualityOptions<object> sut = new();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsFalse()
			.Because("a value that does not fit into the other type must neither wrap around nor lose precision to an equal value");
	}

	[Test]
	[MethodDataSource(nameof(EqualNumbers))]
	public async Task AreConsideredEqual_WhenNumbersHaveSameValue_ShouldReturnTrue(
		object actual, object expected)
	{
		ObjectEqualityOptions<object> sut = new();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsTrue();
	}

	[Test]
	[Arguments(null, "foo")]
	[Arguments("foo", null)]
	public async Task AreConsideredEqual_WhenOnlyOneIsNull_ShouldReturnFalse(string? actual, string? expected)
	{
		ObjectEqualityOptions<string?> sut = new();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsFalse();
	}

	[Test]
	[Arguments(false, "is equal to 3")]
	[Arguments(true, "is not equal to 3")]
	public async Task GetExpectation_ShouldDescribeTheEquality(bool isNegated, string expected)
	{
		ObjectEqualityOptions<int> sut = new();

		string result = sut.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(false, "is equal to 3 using ObjectEqualityOptionsTests.ModuloComparer")]
	[Arguments(true, "is not equal to 3 using ObjectEqualityOptionsTests.ModuloComparer")]
	public async Task GetExpectation_WithATypedComparer_ShouldNameTheComparer(bool isNegated, string expected)
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		string result = sut.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(false, "is equal to 3 using ObjectEqualityOptionsTests.AllEqualComparer")]
	[Arguments(true, "is not equal to 3 using ObjectEqualityOptionsTests.AllEqualComparer")]
	public async Task GetExpectation_WithAnUntypedComparer_ShouldNameTheComparer(bool isNegated, string expected)
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		string result = sut.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task GetItemExpectation_WithATypedComparer_ShouldNameTheComparer()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		string result = sut.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 using ObjectEqualityOptionsTests.ModuloComparer");
	}

	[Test]
	public async Task GetItemExpectation_WithAnUntypedComparer_ShouldNameTheComparer()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		string result = sut.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 using ObjectEqualityOptionsTests.AllEqualComparer");
	}

	[Test]
	public async Task SetMatchType_WithOptionName_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		void Act() => sut.SetMatchType(new DummyMatchType(), "Custom");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Custom cannot be combined with Using.")
			.Because("the match type would silently replace the comparer");
	}

	[Test]
	public async Task SetMatchType_WithOptionName_WhenTheSameOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.SetMatchType(new DummyMatchType(), "Custom");

		void Act() => sut.SetMatchType(new DummyMatchType(), "Custom");

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Custom cannot be specified more than once.")
			.Because("the second match type would silently replace the first one");
	}

	[Test]
	public async Task SetMatchType_WithoutOptionName_ShouldReplaceTheMatchType()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		sut.SetMatchType(new DummyMatchType());

		await That(sut.ToString()).IsEqualTo("dummy")
			.Because("without an option name the match type is set as is, e.g. to resolve a default");
	}

	[Test]
	public async Task Using_WhenAComparerIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be specified more than once.")
			.Because("the second comparer would silently replace the first one");
	}

	[Test]
	public async Task Using_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.SetMatchType(new DummyMatchType(), "Custom");

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Custom.")
			.Because("the comparer would silently replace the match type");
	}

	[Test]
	public async Task Using_WithATypedComparer_ShouldCompareWithIt()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		bool result = await sut.AreConsideredEqual(1, 11);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Using_WithATypedComparer_ShouldNameItsType()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		string? result = sut.ToString();

		await That(result).IsEqualTo(" using ObjectEqualityOptionsTests.ModuloComparer");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenAComparerIsSpecified_ShouldThrowInvalidOperationException()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new AllEqualComparer());

		void Act() => sut.Using(new ModuloComparer(10));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be specified more than once.")
			.Because("the second comparer would silently replace the first one");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheExpectedValueHasAnotherType_ShouldReturnFalse()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		bool result = await sut.AreConsideredEqual(1, 11L);

		await That(result).IsFalse()
			.Because("the comparer can only compare values of its own type");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheExpectedValueIsNullForAValueType_ShouldReturnFalse()
	{
		ObjectEqualityOptions<int> sut = new();
		sut.Using(new ModuloComparer(10));

		bool result = await sut.AreConsideredEqual<object?>(1, null);

		await That(result).IsFalse()
			.Because("null is no value of a value type, so the comparer cannot compare it");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheValuesAreNullForAReferenceType_ShouldCompareWithIt()
	{
		ObjectEqualityOptions<string?> sut = new();
		sut.Using(new AllEqualStringComparer());

		bool result = await sut.AreConsideredEqual(null, (string?)null);

		await That(result).IsTrue()
			.Because("null is a valid value of a reference type, so the comparer decides");
	}

	[Test]
	public async Task Using_WithATypedComparer_WithNull_ShouldThrowArgumentNullException()
	{
		ObjectEqualityOptions<int> sut = new();

		void Act() => sut.Using((IEqualityComparer<int>)null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Using_WithAnUntypedComparer_ShouldCompareWithIt()
	{
		ObjectEqualityOptions<object> sut = new();
		sut.Using(new AllEqualComparer());

		bool result = await sut.AreConsideredEqual(1, "foo");

		await That(result).IsTrue();
	}

	[Test]
	public async Task Using_WithNull_ShouldThrowArgumentNullException()
	{
		ObjectEqualityOptions<object> sut = new();

		void Act() => sut.Using(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
	}

	public static IEnumerable<(object, object)> DifferentNumbers() =>
	[
		(
			-1, uint.MaxValue
		),
		(
			(sbyte)-1, (byte)255
		),
		(
			int.MinValue, 2147483648u
		),
		(
			-1L, ulong.MaxValue
		),
		(
			int.MaxValue, 2147483648f
		),
		(
			1.5, 1
		),
		(
			double.NaN, 0
		),
		(
			double.PositiveInfinity, long.MaxValue
		),
		(
			(nint)(-1), ulong.MaxValue
		),
		(
			(nuint)1, 1.5
		),
#if NET8_0_OR_GREATER
		(
			(Int128)(-1), UInt128.MaxValue
		),
		(
			(nint)(-1), nuint.MaxValue
		),
		(
			(Half)(-1), ushort.MaxValue
		),
		(
			Half.PositiveInfinity, 65536
		),
#endif
	];

	public static IEnumerable<(object, object)> EqualNumbers() =>
	[
		(
			1, 1L
		),
		(
			1.0, 1
		),
		(
			-1, (sbyte)-1
		),
		(
			(decimal)6, 6
		),
		(
			uint.MaxValue, (long)uint.MaxValue
		),
		(
			(nint)1, 1
		),
		(
			(nint)(-2), -2.0
		),
		(
			(nuint)3, (byte)3
		),
		(
			(nint)4, (nuint)4
		),
		(
			(ushort)5, 5L
		),
		(
			7u, (short)7
		),
#if NET8_0_OR_GREATER
		(
			(Half)10, 10
		),
		(
			(Int128)(-1), -1L
		),
		(
			(UInt128)ulong.MaxValue, ulong.MaxValue
		),
#endif
	];

	private sealed class AllEqualComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => true;

		public int GetHashCode(object obj) => 0;
	}

	private sealed class AllEqualStringComparer : IEqualityComparer<string?>
	{
		public bool Equals(string? x, string? y) => true;

		public int GetHashCode(string? obj) => 0;
	}

	private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
	{
		public bool Equals(int x, int y) => x % modulus == y % modulus;

		public int GetHashCode(int obj) => obj % modulus;
	}

	private sealed class ThrowingEquals(Exception exception)
	{
		public override bool Equals(object? obj) => throw exception;

		public override int GetHashCode() => 0;
	}

	private sealed class DummyMatchType : IObjectMatchType
	{
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(true);

		public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
			TExpected expected)
			=> throw new NotSupportedException();

		public string GetExpectation(string expected, ExpectationGrammars grammars) => "";

		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> "";

		public void AppendContexts(ResultContextCollector contexts)
		{
			// The dummy adds no context.
		}

		public override string ToString() => "dummy";
	}
}
