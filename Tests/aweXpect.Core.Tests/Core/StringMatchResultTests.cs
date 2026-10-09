namespace aweXpect.Core.Tests.Core;

public sealed class StringMatchResultTests
{
	[Test]
	public async Task Default_ShouldBeAComparedDifference()
	{
		StringMatchResult result = default;

		await That(result.IsEqual).IsFalse();
		await That(result.NotComparableReason).IsNull()
			.Because("a default value must not fail both ways without a reason");
	}

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task ImplicitConversion_ShouldKeepTheValueAndBeComparable(bool isEqual)
	{
		StringMatchResult result = isEqual;

		await That(result.IsEqual).IsEqualTo(isEqual);
		await That(result.NotComparableReason).IsNull();
		await That(result.Cause).IsNull();
	}

	[Test]
	public async Task NotComparable_ShouldKeepTheReasonAndTheCause()
	{
		FormatException cause = new("no number");

		StringMatchResult result = StringMatchResult.NotComparable("it was no number", cause);

		await That(result.IsEqual).IsFalse();
		await That(result.NotComparableReason).IsEqualTo("it was no number");
		await That(result.Cause).IsSameAs(cause);
	}

	[Test]
	public async Task NotComparable_WhenReasonIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => StringMatchResult.NotComparable(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'reason' cannot be null.").AsPrefix().And
			.WithParamName("reason")
			.Because("without a reason the result would silently count as a compared difference");
	}
}
