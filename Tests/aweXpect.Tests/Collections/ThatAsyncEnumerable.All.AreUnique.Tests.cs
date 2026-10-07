#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenADuplicateIsFoundBeforeTheSourceThrows_ShouldReportTheDuplicate()
				{
					IAsyncEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 1);

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1);

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1, 2, -1);

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
					Func<IAsyncEnumerable<int>?> subject = () => calls++ == 0 ? ToAsyncEnumerable(1, 1) : null;

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.All().AreUnique().WithTimeSystem(new VirtualTimeSystem());

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
					IAsyncEnumerable<int>? subject = null;

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
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(5, cts, token);

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
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1);

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
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

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
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "a",]);

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
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "a", "b", "x",]);

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
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

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

			public sealed class StringElementMemberTests
			{
				[Test]
				public async Task WhenMembersAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "bb", "ccc",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenStringMembersAreNotUnique_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

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
			}

			public sealed class MemberTests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(5, cts, token);

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
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 1, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
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
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable([1, 2, 3, 1, 2, -1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
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
					IAsyncEnumerable<MyClass>? subject = null;

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

			public sealed class NegatedMemberTests
			{
				[Test]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => x.Value));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value not for all items,
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
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => x.Value));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class StringMemberTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "a", "a",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "b", "c",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value ignoring case for all items,
						             but none of at least 2 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "c"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass> subject =
						ToAsyncEnumerable(["a", "b", "c", "a", "b", "x",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique by x => x.Value for all items,
						             but only 2 of at least 4 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "c"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass>? subject = null;

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
#endif
