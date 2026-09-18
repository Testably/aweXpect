using System.Collections;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreNotUnique
		{
			public sealed class EnumerableTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllDifferentComparer());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique using AllDifferentComparer for all items,
						             but none of 3 were

						             Not matching items:
						             [1, 1, 1]

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 1, 2,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 1, 3,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [2, 3]

						             Collection:
						             [1, 2, 1, 3]
						             """);
				}
			}

			public sealed class EnumerableMemberTests
			{
				[Fact]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 1, 2,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => (x as MyClass)?.Value);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerableStringMemberTests
			{
				[Fact]
				public async Task WhenAllMembersAreDuplicatedIgnoringCase_ShouldSucceed()
				{
					IEnumerable subject = new[] { "a", "A", "b", "B", };

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => (string)x!).IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeMembersAreUnique_ShouldFail()
				{
					IEnumerable subject = new[] { "a", "A", "b", "b", };

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => (string)x!);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique by x => (string)x! for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [
						               "a",
						               "A"
						             ]

						             Collection:
						             [
						               "a",
						               "A",
						               "b",
						               "b"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreNotUnique(x => (string)x!);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique by x => (string)x! for all items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
