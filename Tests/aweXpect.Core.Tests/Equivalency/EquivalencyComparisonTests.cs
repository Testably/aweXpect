using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyComparisonTests
{
	[Fact]
	public async Task WhenActualMemberIsMoreVisibleThanRequested_ShouldStillCompareIt()
	{
		WithPublicValue actual = new(1);
		WithInternalValue expected = new(1);
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.Internal,
			Properties = IncludeMembers.None,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(result).IsTrue()
			.Because("the visibility selects the members of the expected object, while the actual side only has to have a member of that name");
	}

	[Fact]
	public async Task WhenAllMembersAreExcludedExplicitly_ShouldNotThrow()
	{
		ClassWithOnlyPrivateState actual = new(1);
		ClassWithOnlyPrivateState expected = new(2);
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.None,
			Properties = IncludeMembers.None,
		};

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, options, new StringBuilder());

		await That(Act).DoesNotThrow()
			.Because("excluding every member is an explicit choice by the caller");
	}

	[Fact]
	public async Task WhenComparedByValue_ShouldReportTheDifferenceInsteadOfThrowing()
	{
		ValueLikeWithoutMembers actual = new(1);
		ValueLikeWithoutMembers expected = new(2);
		StringBuilder failureBuilder = new();
		EquivalencyOptions options = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};

		bool result = await EquivalencyComparison.Compare(actual, expected, options, failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("It differed:")
			.Because("comparing by value is the documented remedy for types without comparable members");
	}

	[Theory]
	[InlineData(1, 3, 2, 3, "Property Value differed")]
	[InlineData(1, 3, 1, 4, "Field Value differed")]
	public async Task WhenFieldHidesAProperty_ShouldCompareBoth(int actualProperty, int actualField,
		int expectedProperty, int expectedField, string expectedDifference)
	{
		FieldHidingProperty actual = new(actualProperty, actualField);
		FieldHidingProperty expected = new(expectedProperty, expectedField);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains(expectedDifference)
			.Because("a field and a property of the same name are both members of the type");
	}

	[Fact]
	public async Task WhenGetterThrows_ShouldThrowTheGetterException()
	{
		WithThrowingGetter actual = new();
		WithThrowingGetter expected = new();

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("getter failed")
			.Because("reflection wraps the exception, while a registered accessor lets it through, so both paths have to agree");
	}

	[Theory]
	[InlineData("foo", "foo", true)]
	[InlineData("foo", "bar", false)]
	public async Task WhenMemberIsHidden_ShouldCompareTheMostDerivedDeclarationOnly(string actualText,
		string expectedText, bool expectedResult)
	{
		PropertyHidingProperty actual = new(1, actualText);
		PropertyHidingProperty expected = new(2, expectedText);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsEqualTo(expectedResult)
			.Because("reflection returns both declarations, but only the one on the most derived type is visible");
	}

	[Fact]
	public async Task WhenNestedMemberHasNoComparableMembers_ShouldIncludeTheMemberPath()
	{
		ClassWithPrivateStateMember actual = new(new ClassWithOnlyPrivateState(1));
		ClassWithPrivateStateMember expected = new(new ClassWithOnlyPrivateState(2));

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Property Inner has no members that could be compared on *")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenNoMembersCanBeCompared_ShouldThrowInvalidOperationException()
	{
		ClassWithOnlyPrivateState actual = new(1);
		ClassWithOnlyPrivateState expected = new(2);

		async Task Act()
			=> await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), new StringBuilder());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"It has no members that could be compared on EquivalencyComparisonTests.ClassWithOnlyPrivateState, which would make the equivalency comparison succeed without verifying anything.*")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenTypeHasAnIndexer_ShouldIgnoreTheIndexer()
	{
		WithIndexer actual = new()
		{
			Count = 1,
		};
		WithIndexer expected = new()
		{
			Count = 2,
		};
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("Property Count differed")
			.Because("an indexer cannot be read without an argument, so it is not a comparable member");
	}

	[Fact]
	public async Task WhenTypesDifferWithoutComparableMembers_ShouldReportTheDifferenceInsteadOfThrowing()
	{
		ClassWithOnlyPrivateState actual = new(1);
		OtherClassWithOnlyPrivateState expected = new(2);
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions(), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).Contains("It differed:")
			.Because("a mismatching type is a difference that can be reported without inspecting members");
	}

	private sealed class ClassWithOnlyPrivateState(int value)
	{
		private readonly int _value = value;

		public override string ToString() => $"{nameof(ClassWithOnlyPrivateState)}({_value})";
	}

	private sealed class ClassWithPrivateStateMember(ClassWithOnlyPrivateState inner)
	{
		public ClassWithOnlyPrivateState Inner { get; } = inner;
	}

	private sealed class FieldHidingProperty(int property, int field) : WithProperty(property)
	{
		public new int Value = field;
	}

	private sealed class OtherClassWithOnlyPrivateState(int value)
	{
		private readonly int _value = value;

		public override string ToString() => $"{nameof(OtherClassWithOnlyPrivateState)}({_value})";
	}

	private sealed class PropertyHidingProperty(int property, string text) : WithProperty(property)
	{
		public new string Value { get; } = text;
	}

	private sealed class ValueLikeWithoutMembers(int value)
	{
		private readonly int _value = value;

		public override bool Equals(object? obj)
			=> obj is ValueLikeWithoutMembers other && other._value == _value;

		public override int GetHashCode() => _value;

		public override string ToString() => $"{nameof(ValueLikeWithoutMembers)}({_value})";
	}

	private sealed class WithIndexer
	{
		public int Count { get; set; }
		public int this[int index] => index;
	}

	private sealed class WithInternalValue(int value)
	{
		internal int Value = value;
	}

	private sealed class WithPublicValue(int value)
	{
		public int Value = value;
	}

	private sealed class WithThrowingGetter
	{
		public int Value => throw new InvalidOperationException("getter failed");
	}

	private class WithProperty(int value)
	{
		public int Value => value;
	}
}
