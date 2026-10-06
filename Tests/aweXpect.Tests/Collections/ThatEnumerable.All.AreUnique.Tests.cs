using System.Collections.Generic;
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
			public sealed class Tests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenADuplicateIsFoundBeforeTheSourceThrows_ShouldReportTheDuplicate()
				{
					IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 1);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but none of at least 2 were

						             Not matching items:
						             [1, 1, (… and maybe more)]

						             Collection:
						             [1, 1, (… and maybe more)]
						             """)
						.Because("the duplicate already decides the result, so the exception of the source must not replace it");
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAnEarlierAttemptHadOtherDuplicates_ShouldDescribeTheLastAttempt()
				{
					int calls = 0;
					Func<int[]> subject = () => calls++ == 0 ? [1, 1,] : [2, 2, 3,];

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             eventually is unique for all items within 0:05,
						             but only 1 of 3 were

						             Not matching items:
						             [2, 2]

						             Collection:
						             [2, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenDateTimesOnlyDifferInTheirKind_ShouldSucceed()
				{
					DateTime local = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Local);
					DateTime utc = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					IEnumerable<DateTime> subject = ToEnumerable([local, utc,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow()
						.Because("both have the same hash code, but the comparison still decides that their kinds are incompatible");
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

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
				public async Task WhenItContainsEquivalentDuplicates_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable([new MyClass(1), new MyClass(2), new MyClass(1),]);

					async Task Act()
						=> await That(subject).All().AreUnique().Equivalent();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique using equivalency for all items,
						             but only 1 of at least 3 were

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
						                 Value = 1
						               },
						               (… and maybe more)
						             ]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Test]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1, 2, -1,]);

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
				public async Task WhenSubjectIsNull_AfterAnEarlierAttempt_ShouldNotShowItsItems()
				{
					int calls = 0;
					Func<IEnumerable<int>?> subject = () => calls++ == 0 ? [1, 1,] : null;

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             eventually is unique for all items within 0:05,
						             but it was <null>
						             """)
						.Because("the items of an earlier attempt do not describe the last one");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class ItemTests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

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
				public async Task ConsidersCancellationToken_WhenNested_ShouldNameTheMember()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					Container subject = new(GetCancellingEnumerable(5, cts));

					async Task Act()
						=> await That(subject).Whose(c => c.Items, items => items.All().AreUnique())
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             whose Items are unique for all items,
						             but Items could not be verified, because the evaluation was already canceled
						             *
						             """).AsWildcard();
				}

				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().AreUnique()
							.And.All().AreUnique();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

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
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class StringTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "a", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique().IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique ignoring case for all items,
						             but none of at least 2 were

						             Not matching items:
						             [
						               "a",
						               "A",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "a",
						               "A",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [
						               "a",
						               "a",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "a",
						               "b",
						               "c",
						               "a",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenMembersAreUnique_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "bb", "ccc",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenStringMembersAreNotUnique_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!).IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x! ignoring case for all items,
						             but none of at least 2 were

						             Not matching items:
						             [
						               "a",
						               "A",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "a",
						               "A",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class MemberTests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x * 2).WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x * 2 for all items,
						             but it could not be verified, because the evaluation was already canceled
						             *
						             """).AsWildcard();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 1, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
						             but only 1 of at least 3 were

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
						                 Value = 1
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringMemberTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "a", "a",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value ignoring case for all items,
						             but none of at least 2 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<MyStringClass>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
						             but it was <null>
						             """);
				}

				private sealed class MyStringClass(string value)
				{
					public string Value { get; } = value;
				}
			}
		}
	}
}
