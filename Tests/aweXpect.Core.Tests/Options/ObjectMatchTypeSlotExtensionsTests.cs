using System.Collections.Generic;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ObjectMatchTypeSlotExtensionsTests
{
	[Test]
	public async Task AreConsideredEqual_WhenBothAreNull_ShouldReturnTrue()
	{
		IEqualityEvaluator<string?> sut = DefaultEvaluator<string?>();

		bool result = await sut.AreConsideredEqual(null, (string?)null);

		await That(result).IsTrue();
	}

	[Test]
	public async Task AreConsideredEqual_WhenEqualsThrowsForAnotherType_ShouldNameTheEqualsOfTheSubject()
	{
		InvalidOperationException exception = new("equals failed");
		IEqualityEvaluator<ThrowingEquals> sut = DefaultEvaluator<ThrowingEquals>();

		async Task Act()
			=> await sut.AreConsideredEqual(new ThrowingEquals(exception), "foo");

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("Equals of ObjectMatchTypeSlotExtensionsTests.ThrowingEquals")).And
			.Whose(e => e.Exception, inner => inner.IsSameAs(exception))
			.Because("a value of another type is compared with Equals(object) of the subject");
	}

	[Test]
	public async Task AreConsideredEqual_WhenEqualsThrows_ShouldNameTheEqualsOfTheSubject()
	{
		InvalidOperationException exception = new("equals failed");
		IEqualityEvaluator<ThrowingEquals> sut = DefaultEvaluator<ThrowingEquals>();

		async Task Act()
			=> await sut.AreConsideredEqual(new ThrowingEquals(exception), new ThrowingEquals(exception));

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("Equals of ObjectMatchTypeSlotExtensionsTests.ThrowingEquals")).And
			.Whose(e => e.Exception, inner => inner.IsSameAs(exception));
	}

	[Test]
	[MethodDataSource(nameof(DifferentNumbers))]
	public async Task AreConsideredEqual_WhenNumbersHaveDifferentValues_ShouldReturnFalse(
		object actual, object expected)
	{
		IEqualityEvaluator<object> sut = DefaultEvaluator<object>();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsFalse()
			.Because("a value that does not fit into the other type must neither wrap around nor lose precision to an equal value");
	}

	[Test]
	[MethodDataSource(nameof(EqualNumbers))]
	public async Task AreConsideredEqual_WhenNumbersHaveSameValue_ShouldReturnTrue(
		object actual, object expected)
	{
		IEqualityEvaluator<object> sut = DefaultEvaluator<object>();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsTrue();
	}

	[Test]
	[Arguments(null, "foo")]
	[Arguments("foo", null)]
	public async Task AreConsideredEqual_WhenOnlyOneIsNull_ShouldReturnFalse(string? actual, string? expected)
	{
		IEqualityEvaluator<string?> sut = DefaultEvaluator<string?>();

		bool result = await sut.AreConsideredEqual(actual, expected);

		await That(result).IsFalse();
	}

	[Test]
	[Arguments(180, true)]
	[Arguments(181, false)]
	public async Task AreConsideredEqual_WhenTheSubjectIsAValueType_ShouldCompareTheValues(int expectedSeconds,
		bool expectedResult)
	{
		IEqualityEvaluator<TrackLength> sut = DefaultEvaluator<TrackLength>();

		bool result = await sut.AreConsideredEqual(new TrackLength(180), new TrackLength(expectedSeconds));

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	[Arguments("Something", true)]
	[Arguments("Yesterday", false)]
	public async Task AreConsideredEqual_WhenTheSubjectOverridesEquals_ShouldUseIt(string expectedTitle,
		bool expectedResult)
	{
		IEqualityEvaluator<Track> sut = DefaultEvaluator<Track>();

		bool result = await sut.AreConsideredEqual(new Track("Something"), new Track(expectedTitle));

		await That(result).IsEqualTo(expectedResult)
			.Because("different instances are equal when their Equals says so");
	}

	[Test]
	public async Task Default_ShouldBeSharedAndHaveNoOptionName()
	{
		ObjectMatchType<int> first = ObjectMatchType<int>.Default;
		ObjectMatchType<int> second = ObjectMatchType<int>.Default;

		await That(second).IsSameAs(first);
		await That(first.OptionName).IsNull();
		await That(first.ToString()).IsEmpty();
	}

	[Test]
	[Arguments(false, "is equal to 3")]
	[Arguments(true, "is not equal to 3")]
	public async Task GetExpectation_ShouldDescribeTheEquality(bool isNegated, string expected)
	{
		ObjectMatchType<int> sut = ObjectMatchType<int>.Default;

		string result = sut.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(false, "is equal to 3 using ObjectMatchTypeSlotExtensionsTests.ModuloComparer")]
	[Arguments(true, "is not equal to 3 using ObjectMatchTypeSlotExtensionsTests.ModuloComparer")]
	public async Task GetExpectation_WithATypedComparer_ShouldNameTheComparer(bool isNegated, string expected)
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		string result =
			sut.Value.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(false, "is equal to 3 using ObjectMatchTypeSlotExtensionsTests.AllEqualComparer")]
	[Arguments(true, "is not equal to 3 using ObjectMatchTypeSlotExtensionsTests.AllEqualComparer")]
	public async Task GetExpectation_WithAnUntypedComparer_ShouldNameTheComparer(bool isNegated, string expected)
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.Using(new AllEqualComparer());

		string result =
			sut.Value.GetExpectation("3", isNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task GetItemExpectation_WithATypedComparer_ShouldNameTheComparer()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		string result = sut.Value.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 using ObjectMatchTypeSlotExtensionsTests.ModuloComparer");
	}

	[Test]
	public async Task GetItemExpectation_WithAnUntypedComparer_ShouldNameTheComparer()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.Using(new AllEqualComparer());

		string result = sut.Value.GetItemExpectation("3", "an item", "equal to");

		await That(result).IsEqualTo("an item equal to 3 using ObjectMatchTypeSlotExtensionsTests.AllEqualComparer");
	}

	[Test]
	public async Task MatchWithExplanation_WhenEqual_ShouldReturnAMatch()
	{
		ObjectMatcher<int> sut = ObjectMatchType<int>.Default.ForEvaluation(default);

		MatchResult result = await sut.CompareWithExplanation(1, 1);

		await That(result.IsMatch).IsTrue();
	}

	[Test]
	public async Task MatchWithExplanation_WhenNotEqual_ShouldExplainWithTheActualValue()
	{
		ObjectMatcher<int> sut = ObjectMatchType<int>.Default.ForEvaluation(default);

		MatchResult result = await sut.CompareWithExplanation(1, 2);

		await That(result.IsMatch).IsFalse();
		await That(sut.GetFailure("it", ExpectationGrammars.None, 1, 2, result, null)).IsEqualTo("it was 1");
	}

	[Test]
	public async Task MatchWithExplanation_WhenNotEqual_WithIndentation_ShouldIndentTheActualValue()
	{
		ObjectMatcher<object> sut = ObjectMatchType<object>.Default.ForEvaluation(default);
		object actual = new
		{
			Value = 1,
		};

		MatchResult result = await sut.CompareWithExplanation(actual, 2);

		await That(sut.GetFailure("it", ExpectationGrammars.None, actual, 2, result, "      "))
			.IsEqualTo($"it was {Formatter.Format(actual, FormattingOptions.Indented("      "))}")
			.Because("the lines of the value are indented like the result they belong to");
	}

	[Test]
	[Arguments(11, true)]
	[Arguments(12, false)]
	public async Task MatchWithExplanation_WithATypedComparer_ShouldDecideWithIt(int expected,
		bool expectMatch)
	{
		OptionsSlot<ObjectMatchType<int>> slot = new(ObjectMatchType<int>.Default);
		slot.Using(new ModuloComparer(10));
		ObjectMatcher<int> sut = slot.Value.ForEvaluation(default);

		MatchResult result = await sut.CompareWithExplanation(1, expected);

		await That(result.IsMatch).IsEqualTo(expectMatch);
		await That(sut.GetFailure("it", ExpectationGrammars.None, 1, expected, result, null)).IsEqualTo("it was 1");
	}

	[Test]
	public async Task MatchWithExplanation_WithAnUntypedComparer_ShouldDecideWithIt()
	{
		OptionsSlot<ObjectMatchType<object>> slot = new(ObjectMatchType<object>.Default);
		slot.Using(new AllEqualComparer());

		MatchResult result = await slot.Value.ForEvaluation(default).CompareWithExplanation(1, "foo");

		await That(result.IsMatch).IsTrue();
	}

	[Test]
	public async Task SetMatchType_ShouldNotChangeTheMatchTypeThatWasReplaced()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		ObjectMatchType<object> stored = sut.Value;

		sut.SetMatchType(new DummyMatchType());

		await That(stored).IsSameAs(ObjectMatchType<object>.Default);
		await That(stored.OptionName).IsNull();
		await That(sut.Value.OptionName).IsEqualTo("Custom");
	}

	[Test]
	public async Task SetMatchType_ShouldReturnTheSameSlot()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);

		OptionsSlot<ObjectMatchType<object>> result = sut.SetMatchType(new DummyMatchType());

		await That(result).IsSameAs(sut);
	}

	[Test]
	public async Task SetMatchType_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.Using(new AllEqualComparer());

		void Act() => sut.SetMatchType(new DummyMatchType());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Custom cannot be combined with Using.")
			.Because("the match type would silently replace the comparer");
	}

	[Test]
	public async Task SetMatchType_WhenTheSameOptionIsSpecified_ShouldReplaceTheMatchType()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		DummyMatchType second = new();
		sut.SetMatchType(new DummyMatchType());

		void Act() => sut.SetMatchType(second);

		await That(Act).DoesNotThrow();
		await That(sut.Value).IsSameAs(second);
		await That(() => sut.Using(new AllEqualComparer())).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Custom.")
			.Because("the replaced match type still competes with another option");
	}

	[Test]
	public async Task SetMatchType_WithNull_ShouldThrowArgumentNullException()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);

		void Act() => sut.SetMatchType(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("matchType").And
			.WithMessage("The 'matchType' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task SetMatchType_WithoutOptionName_ShouldThrowArgumentException()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);

		void Act() => sut.SetMatchType(new UnnamedMatchType());

		await That(Act).Throws<ArgumentException>()
			.WithParamName("matchType").And
			.WithMessage("The match type must have an option name.").AsPrefix();
		await That(sut.Value).IsSameAs(ObjectMatchType<object>.Default);
	}

	[Test]
	public async Task Using_ShouldNameTheOption()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);

		sut.Using(new ModuloComparer(10));

		await That(sut.Value.OptionName).IsEqualTo("Using");
	}

	[Test]
	public async Task Using_WhenAComparerIsSpecified_ShouldUseTheLastComparer()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.Using(new AllEqualComparer());

		sut.Using(EqualityComparer<object>.Default);

		await That(await Evaluator(sut).AreConsideredEqual(1, "foo")).IsFalse();
	}

	[Test]
	public async Task Using_WhenAnotherOptionIsSpecified_ShouldThrowInvalidOperationException()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.SetMatchType(new DummyMatchType());

		void Act() => sut.Using(new AllEqualComparer());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with Custom.")
			.Because("the comparer would silently replace the match type");
	}

	[Test]
	public async Task Using_WhenTheDefaultIsShared_ShouldNotChangeAnotherSlot()
	{
		OptionsSlot<ObjectMatchType<int>> first = new(ObjectMatchType<int>.Default);
		OptionsSlot<ObjectMatchType<int>> second = new(ObjectMatchType<int>.Default);

		first.Using(new ModuloComparer(10));

		await That(second.Value).IsSameAs(ObjectMatchType<int>.Default);
		await That(await Evaluator(second).AreConsideredEqual(1, 11)).IsFalse()
			.Because("the comparer of one expectation must not reach another one");
	}

	[Test]
	public async Task Using_WithATypedComparer_ShouldCompareWithIt()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		bool result = await Evaluator(sut).AreConsideredEqual(1, 11);

		await That(result).IsTrue();
	}

	[Test]
	public async Task Using_WithATypedComparer_ShouldNameItsType()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		string? result = sut.ToString();

		await That(result).IsEqualTo(" using ObjectMatchTypeSlotExtensionsTests.ModuloComparer");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenAComparerIsSpecified_ShouldUseTheLastComparer()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new AllEqualComparer());

		sut.Using(new ModuloComparer(10));

		await That(await Evaluator(sut).AreConsideredEqual(1, 2)).IsFalse();
		await That(await Evaluator(sut).AreConsideredEqual(1, 11)).IsTrue();
		await That(sut.ToString()).IsEqualTo(" using ObjectMatchTypeSlotExtensionsTests.ModuloComparer");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheExpectedValueHasAnotherType_ShouldReturnFalse()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		bool result = await Evaluator(sut).AreConsideredEqual(1, 11L);

		await That(result).IsFalse()
			.Because("the comparer can only compare values of its own type");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheExpectedValueIsNullForAValueType_ShouldReturnFalse()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);
		sut.Using(new ModuloComparer(10));

		bool result = await Evaluator(sut).AreConsideredEqual<object?>(1, null);

		await That(result).IsFalse()
			.Because("null is no value of a value type, so the comparer cannot compare it");
	}

	[Test]
	public async Task Using_WithATypedComparer_WhenTheValuesAreNullForAReferenceType_ShouldCompareWithIt()
	{
		OptionsSlot<ObjectMatchType<string?>> sut = new(ObjectMatchType<string?>.Default);
		sut.Using(new AllEqualStringComparer());

		bool result = await Evaluator(sut).AreConsideredEqual(null, (string?)null);

		await That(result).IsTrue()
			.Because("null is a valid value of a reference type, so the comparer decides");
	}

	[Test]
	public async Task Using_WithATypedComparer_WithNull_ShouldThrowArgumentNullException()
	{
		OptionsSlot<ObjectMatchType<int>> sut = new(ObjectMatchType<int>.Default);

		void Act() => sut.Using((IEqualityComparer<int>)null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("comparer").And
			.WithMessage("The 'comparer' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Using_WithAnUntypedComparer_ShouldCompareWithIt()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);
		sut.Using(new AllEqualComparer());

		bool result = await Evaluator(sut).AreConsideredEqual(1, "foo");

		await That(result).IsTrue();
	}

	[Test]
	public async Task Using_WithNull_ShouldThrowArgumentNullException()
	{
		OptionsSlot<ObjectMatchType<object>> sut = new(ObjectMatchType<object>.Default);

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

	private sealed class Track(string title)
	{
		public string Title { get; } = title;

		public override bool Equals(object? obj) => obj is Track other && other.Title == Title;

		public override int GetHashCode() => Title.GetHashCode();
	}

	private readonly struct TrackLength(int seconds)
	{
		public int Seconds { get; } = seconds;
	}

#pragma warning disable CA1859 // The ObjectMatcher implements AreConsideredEqual explicitly, so the tests need the interface
	private static IEqualityEvaluator<T> DefaultEvaluator<T>()
		=> ObjectMatchType<T>.Default.ForEvaluation(default);

	private static IEqualityEvaluator<T> Evaluator<T>(OptionsSlot<ObjectMatchType<T>> sut)
		=> sut.Value.ForEvaluation(default);
#pragma warning restore CA1859

	private sealed class DummyMatchType() : ObjectMatchType<object>("Custom")
	{
		public override ObjectMatcher<object> ForEvaluation(MatchEnvironment environment)
			=> throw new NotSupportedException();

		public override MatchPhrase Phrase { get; } = MatchPhrase.EqualTo.WithName("dummy");
	}

	private sealed class UnnamedMatchType : ObjectMatchType<object>
	{
		public override ObjectMatcher<object> ForEvaluation(MatchEnvironment environment)
			=> throw new NotSupportedException();

		public override MatchPhrase Phrase { get; } = MatchPhrase.EqualTo.WithName("unnamed");
	}
}