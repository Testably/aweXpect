using System.Collections;
using System.Linq;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreUnique
		{
			public sealed class EnumerableTests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().AreUnique().WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it could not be verified, because the evaluation was already canceled
						             *
						             """).AsWildcard();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [1, 1, (… and maybe more)]

						             Collection:
						             [1, 2, 3, 1, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1, 2, -1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [1, 1, (… and maybe more)]

						             Collection:
						             [1, 2, 3, 1, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenItContainsMultipleNullItems_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<object?>([null, null,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but none of at least 2 were

						             Not matching items:
						             [
						               <null>,
						               <null>,
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               <null>,
						               <null>,
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedTests
			{
				[Test]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique not for all items,
						             but all 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerableMemberTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (x as MyClass)?.Value for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable subject =
						ToEnumerable([1, 2, 3, 1, 2, -1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (x as MyClass)?.Value for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreUnique(x => (x as MyClass)?.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (x as MyClass)?.Value for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedMemberTests
			{
				[Test]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => (x as MyClass)?.Value));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (x as MyClass)?.Value not for all items,
						             but all 3 were

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               }
						             ]
						             """);
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 2, 3, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => (x as MyClass)?.Value));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class EnumerableStringMemberTests
			{
				[Test]
				public async Task WhenAllMembersAreUnique_ShouldSucceed()
				{
					IEnumerable subject = new[] { "a", "b", "c", };

					async Task Act()
						=> await That(subject).All().AreUnique(x => (string)x!);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMembersDifferInCasing_ShouldSucceed()
				{
					IEnumerable subject = new[] { "a", "A", };

					async Task Act()
						=> await That(subject).All().AreUnique(x => (string)x!);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMembersDifferInCasingAndCasingIsIgnored_ShouldFail()
				{
					IEnumerable subject = new[] { "a", "b", "A", };

					async Task Act()
						=> await That(subject).All().AreUnique(x => (string)x!).IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (string)x! ignoring case for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               "a",
						               "A"
						             ]

						             Collection:
						             [
						               "a",
						               "b",
						               "A"
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().AreUnique(x => (string)x!);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (string)x! for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedStringMemberTests
			{
				[Test]
				public async Task WhenAllMembersAreUnique_ShouldFail()
				{
					IEnumerable subject = new[] { "a", "b", };

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => (string)x!));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => (string)x! not for all items,
						             but all 2 were

						             Collection:
						             [
						               "a",
						               "b"
						             ]
						             """);
				}

				[Test]
				public async Task WhenMembersDifferOnlyInCasingAndCasingIsIgnored_ShouldSucceed()
				{
					IEnumerable subject = new[] { "a", "A", };

					async Task Act()
						=> await That(subject)
							.DoesNotComplyWith(it => it.All().AreUnique(x => (string)x!).IgnoringCase());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
