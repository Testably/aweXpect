#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1);

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1, 2, -1);

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
					IAsyncEnumerable<int>? subject = null;

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
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(5, cts, token);

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
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

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
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1);

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
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "a",]);

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

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
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "a",]);

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
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "a", "b", "x",]);

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [
						               "a",
						               "b",
						               "a",
						               "b"
						             ]

						             Collection:
						             [
						               "a",
						               "b",
						               "c",
						               "a",
						               "b",
						               "x"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

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

			public sealed class StringElementMemberTests
			{
				[Fact]
				public async Task WhenMembersAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "bb", "ccc",]);

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenStringMembersAreNotUnique_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

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
			}

			public sealed class MemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 1, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3, 1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 4 were

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
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable([1, 2, 3, 1, 2, -1,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 6 were

						             Not matching items:
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
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = -1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<MyClass>? subject = null;

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

			public sealed class NegatedMemberTests
			{
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => x.Value));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for x => x.Value for all items,
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

				[Fact]
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
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "a", "a",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "b", "c",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value ignoring case for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]

						             Collection:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass>
						subject = ToAsyncEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               }
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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass> subject =
						ToAsyncEnumerable(["a", "b", "c", "a", "b", "x",], x => new MyStringClass(x));

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "b"
						               }
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
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatAsyncEnumerable.All.AreUnique.StringMemberTests.MyStringClass {
						                 Value = "x"
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<MyStringClass>? subject = null;

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
#endif
