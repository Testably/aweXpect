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
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [1, 1]

						             Collection:
						             [1, 2, 3, 1]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1, 2, -1,]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [1, 2, 1, 2]

						             Collection:
						             [1, 2, 3, 1, 2, -1]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class ItemTests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().AreUnique().WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but could not verify, because it was already cancelled
						             *
						             """).AsWildcard();
				}

				[Fact]
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
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but all 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
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
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "a", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique().IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique ignoring case for all items,
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

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [
						               "a",
						               "a"
						             ]

						             Collection:
						             [
						               "a",
						               "b",
						               "c",
						               "a"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenMembersAreUnique_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "bb", "ccc",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenStringMembersAreNotUnique_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!).IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x! ignoring case for all items,
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

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class MemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 1, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable([1, 2, 1,]).Select(x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
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
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringMemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "a", "a",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",])
						.Select(x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value ignoring case for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<MyStringClass>? subject = null;

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
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
