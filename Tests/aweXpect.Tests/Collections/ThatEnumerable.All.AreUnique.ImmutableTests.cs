#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreUnique
		{
			public sealed class ImmutableArrayTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [1, 1, 1,];

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 3, 1,];

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
					ImmutableArray<int> subject = [1, 2, 3, 1, 2, -1,];

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
			}

			public sealed class ImmutableArrayNegatedTests
			{
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 3,];

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
					ImmutableArray<int> subject = [1, 2, 3, 1,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableArrayStringTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<string?> subject = ["a", "a", "a",];

					async Task Act()
						=> await That(subject).All().AreUnique().Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "b", "c",];

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "A",];

					async Task Act()
						=> await That(subject).All().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					ImmutableArray<string?> subject = ["a", "A",];

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
					ImmutableArray<string?> subject = ["a", "b", "c", "a",];

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
					ImmutableArray<string?> subject = ["a", "b", "c", "a", "b", "x",];

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
			}

			public sealed class ImmutableArrayStringElementMemberTests
			{
				[Fact]
				public async Task WhenMembersAreUnique_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "bb", "ccc",];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenStringMembersAreNotUnique_ShouldFail()
				{
					ImmutableArray<string?> subject = ["a", "A",];

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

			public sealed class ImmutableArrayMemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(1), new MyClass(1),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(2), new MyClass(3),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					ImmutableArray<MyClass> subject =
						[new MyClass(1), new MyClass(2), new MyClass(3), new MyClass(1),];

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
					ImmutableArray<MyClass> subject =
					[
						new MyClass(1), new MyClass(2), new MyClass(3), new MyClass(1), new MyClass(2), new MyClass(-1),
					];

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

			}

			public sealed class ImmutableArrayNegatedMemberTests
			{
				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(2), new MyClass(3),];

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
					ImmutableArray<MyClass> subject =
						[new MyClass(1), new MyClass(2), new MyClass(3), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreUnique(x => x.Value));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableArrayStringMemberTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<MyStringClass> subject =
						[new MyStringClass("a"), new MyStringClass("a"), new MyStringClass("a"),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).Using(new AllDifferentComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldSucceed()
				{
					ImmutableArray<MyStringClass> subject =
						[new MyStringClass("a"), new MyStringClass("b"), new MyStringClass("c"),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasing_ShouldSucceed()
				{
					ImmutableArray<MyStringClass> subject = [new MyStringClass("a"), new MyStringClass("A"),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
				{
					ImmutableArray<MyStringClass> subject = [new MyStringClass("a"), new MyStringClass("A"),];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value).IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value ignoring case for all items,
						             but none of 2 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "A"
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldFail()
				{
					ImmutableArray<MyStringClass> subject =
					[
						new MyStringClass("a"), new MyStringClass("b"), new MyStringClass("c"), new MyStringClass("a"),
					];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               }
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "c"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItContainsMultipleDuplicates_ShouldFail()
				{
					ImmutableArray<MyStringClass> subject =
					[
						new MyStringClass("a"), new MyStringClass("b"), new MyStringClass("c"), new MyStringClass("a"),
						new MyStringClass("b"), new MyStringClass("x"),
					];

					async Task Act()
						=> await That(subject).All().AreUnique(x => x.Value);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for x => x.Value for all items,
						             but only 2 of 6 were

						             Not matching items:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "b"
						               }
						             ]

						             Collection:
						             [
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "c"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "a"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "b"
						               },
						               ThatEnumerable.All.AreUnique.ImmutableArrayStringMemberTests.MyStringClass {
						                 Value = "x"
						               }
						             ]
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
