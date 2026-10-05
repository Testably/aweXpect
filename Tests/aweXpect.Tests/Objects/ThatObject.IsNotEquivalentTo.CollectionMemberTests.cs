namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsNotEquivalentTo
	{
		public sealed class CollectionMemberTests
		{
			[Test]
			public async Task WhenCollectionsHaveEqualItemsAndMembers_ShouldFail()
			{
				IsEquivalentTo.CollectionMemberTests.PagedResult<string> subject = new(5, "a", "b");
				IsEquivalentTo.CollectionMemberTests.PagedResult<string> unexpected = new(5, "a", "b");

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to unexpected,
					             but it was [
					                 "a",
					                 "b"
					               ], which is considered equivalent

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenCollectionsOnlyDifferInAMember_ShouldSucceed()
			{
				IsEquivalentTo.CollectionMemberTests.PagedResult<string> subject = new(5, "a", "b");
				IsEquivalentTo.CollectionMemberTests.PagedResult<string> unexpected = new(7, "a", "b");

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the members that the collection type declares itself are compared in addition to the items");
			}
		}
	}
}
