using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreNotUnique
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllDifferentComparer());

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 2,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 3,]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllItemsAreDuplicated_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 2,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNotUnique());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique not for all items,
						             but all 4 were

						             Collection:
						             [1, 2, 1, 2]
						             """);
				}

				[Test]
				public async Task WhenSomeItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNotUnique());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class StringTests
			{
				[Test]
				public async Task WhenDiffersInCasing_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               "a",
						               "A"
						             ]

						             Collection:
						             [
						               "a",
						               "A"
						             ]
						             """);
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique().IgnoringCase();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class StringMemberTests
			{
				[Test]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "cc", "dd",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeStringMembersAreUnique_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A", "b",]);

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!).IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique by x => x! ignoring case for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               "b"
						             ]

						             Collection:
						             [
						               "a",
						               "A",
						               "b"
						             ]
						             """);
				}
			}

			public sealed class MemberTests
			{
				[Test]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2, 1, 2,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2,], x => new MyClass(x, "a"));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.StringValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeMembersAreUnique_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 1, 2,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique by x => x.Value for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               }
						             ]
						             """);
				}
			}
		}
	}
}
