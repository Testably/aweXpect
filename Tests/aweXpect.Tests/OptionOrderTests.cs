namespace aweXpect.Tests;

public sealed class OptionOrderTests
{
	[Fact]
	public async Task DoubleCollectionIsEqualTo_InAnyOrderBeforeWithin_ShouldSucceed()
	{
		double[] subject = [1.0, 2.0,];

		async Task Act()
			=> await That(subject).IsEqualTo(new[] { 2.05, 1.05, }).InAnyOrder().Within(0.1);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task DoubleCollectionIsEqualTo_InAnyOrderBeforeWithin_WhenOutsideTheTolerance_ShouldFailLikeTheReversedOrder()
	{
		double[] subject = [1.0, 2.0,];
		string expectedMessage = """
		                         Expected that subject
		                         is equal to collection new[] { 2.05, 1.05, } ± 0.01 in any order,
		                         but it
		                           contained item 1.0 at index 0 that was not expected and
		                           contained item 2.0 at index 1 that was not expected and
		                           lacked all 2 expected items

		                         Collection:
		                         [1.0, 2.0]

		                         Expected:
		                         [2.05, 1.05]
		                         """;

		async Task Act()
			=> await That(subject).IsEqualTo(new[] { 2.05, 1.05, }).InAnyOrder().Within(0.01);

		async Task ReversedAct()
			=> await That(subject).IsEqualTo(new[] { 2.05, 1.05, }).Within(0.01).InAnyOrder();

		await That(Act).Throws<XunitException>().WithMessage(expectedMessage);
		await That(ReversedAct).Throws<XunitException>().WithMessage(expectedMessage);
	}

	[Fact]
	public async Task StringCollectionIsEqualTo_IgnoringCaseBeforeInAnyOrder_ShouldSucceed()
	{
		string[] subject = ["a", "b",];

		async Task Act()
			=> await That(subject).IsEqualTo(new[] { "B", "A", }).IgnoringCase().InAnyOrder();

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task StringCollectionIsEqualTo_IgnoringCaseBeforeInAnyOrder_WhenNotMatching_ShouldFailLikeTheReversedOrder()
	{
		string[] subject = ["a", "b",];
		string expectedMessage = """
		                         Expected that subject
		                         is equal to collection new[] { "B", "C", } ignoring case in any order,
		                         but it
		                           contained item "a" at index 0 that was not expected and
		                           lacked 1 of 2 expected items: "C"

		                         Collection:
		                         [
		                           "a",
		                           "b"
		                         ]

		                         Expected:
		                         [
		                           "B",
		                           "C"
		                         ]
		                         """;

		async Task Act()
			=> await That(subject).IsEqualTo(new[] { "B", "C", }).IgnoringCase().InAnyOrder();

		async Task ReversedAct()
			=> await That(subject).IsEqualTo(new[] { "B", "C", }).InAnyOrder().IgnoringCase();

		await That(Act).Throws<XunitException>().WithMessage(expectedMessage);
		await That(ReversedAct).Throws<XunitException>().WithMessage(expectedMessage);
	}

	[Fact]
	public async Task StringContains_AtLeastBeforeAsPrefix_ShouldFailLikeTheReversedOrder()
	{
		string subject = "abc";
		string expectedMessage = """
		                         Expected that subject
		                         contains "a" as prefix at least twice,
		                         but it contained "a" once in "abc"
		                         """;

		async Task Act()
			=> await That(subject).Contains("a").AtLeast(2).AsPrefix();

		async Task ReversedAct()
			=> await That(subject).Contains("a").AsPrefix().AtLeast(2);

		await That(Act).Throws<XunitException>().WithMessage(expectedMessage);
		await That(ReversedAct).Throws<XunitException>().WithMessage(expectedMessage);
	}

	[Fact]
	public async Task StringHasItem_AtIndexBeforeIgnoringCase_ShouldSucceed()
	{
		string[] subject = ["a", "b",];

		async Task Act()
			=> await That(subject).HasItem("A").AtIndex(0).IgnoringCase();

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task StringHasItem_AtIndexBeforeIgnoringCase_WhenNotMatching_ShouldFailLikeTheReversedOrder()
	{
		string[] subject = ["a", "b",];
		string expectedMessage = """
		                         Expected that subject
		                         has an item equal to "B" ignoring case at index 0,
		                         but it had item "a" at index 0

		                         Collection:
		                         [
		                           "a",
		                           "b"
		                         ]
		                         """;

		async Task Act()
			=> await That(subject).HasItem("B").AtIndex(0).IgnoringCase();

		async Task ReversedAct()
			=> await That(subject).HasItem("B").IgnoringCase().AtIndex(0);

		await That(Act).Throws<XunitException>().WithMessage(expectedMessage);
		await That(ReversedAct).Throws<XunitException>().WithMessage(expectedMessage);
	}

	[Fact]
	public async Task StringIsEqualTo_IgnoringCaseBeforeAsWildcard_ShouldSucceed()
	{
		string subject = "abc";

		async Task Act()
			=> await That(subject).IsEqualTo("A*").IgnoringCase().AsWildcard();

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task StringIsEqualTo_IgnoringCaseBeforeAsWildcard_WhenNotMatching_ShouldFailLikeTheReversedOrder()
	{
		string subject = "abc";
		string expectedMessage = """
		                         Expected that subject
		                         matches "B*" ignoring case,
		                         but it did not match:
		                           ↓ (actual)
		                           "abc"
		                           "B*"
		                           ↑ (wildcard pattern)
		                         """;

		async Task Act()
			=> await That(subject).IsEqualTo("B*").IgnoringCase().AsWildcard();

		async Task ReversedAct()
			=> await That(subject).IsEqualTo("B*").AsWildcard().IgnoringCase();

		await That(Act).Throws<XunitException>().WithMessage(expectedMessage);
		await That(ReversedAct).Throws<XunitException>().WithMessage(expectedMessage);
	}
}
