using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyComparisonTests
{
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

	private sealed class OtherClassWithOnlyPrivateState(int value)
	{
		private readonly int _value = value;

		public override string ToString() => $"{nameof(OtherClassWithOnlyPrivateState)}({_value})";
	}

	private sealed class ValueLikeWithoutMembers(int value)
	{
		private readonly int _value = value;

		public override bool Equals(object? obj)
			=> obj is ValueLikeWithoutMembers other && other._value == _value;

		public override int GetHashCode() => _value;

		public override string ToString() => $"{nameof(ValueLikeWithoutMembers)}({_value})";
	}
}
