#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AreAllUnique
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<int> subject = [1, 1, 1,];

				async Task Act()
					=> await That(subject).AreAllUnique().Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItContainsDuplicates_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3, 1,];

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it contained 1 duplicate:
					               1

					             Collection:
					             [1, 2, 3, 1]
					             """);
			}

			[Fact]
			public async Task WhenItContainsMultipleDuplicates_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3, 1, 2, -1,];

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it contained 2 duplicates:
					               1,
					               2

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
					=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has duplicate items,
					             but all were unique

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenItContainsDuplicates_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3, 1,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique());

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
					=> await That(subject).AreAllUnique().Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasing_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).AreAllUnique().IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items ignoring case,
					             but it contained 1 duplicate:
					               "A"

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
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it contained 1 duplicate:
					               "a"

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
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it contained 2 duplicates:
					               "a",
					               "b"

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

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(1), new MyClass(1),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(2), new MyClass(3),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItContainsDuplicates_ShouldFail()
			{
				ImmutableArray<MyClass> subject =
					[new MyClass(1), new MyClass(2), new MyClass(3), new MyClass(1),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
					             but it contained 1 duplicate:
					               1

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
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
					             but it contained 2 duplicates:
					               1,
					               2

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
					=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique(x => x.Value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has duplicate items for x => x.Value,
					             but all were unique

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
					=> await That(subject).DoesNotComplyWith(it => it.AreAllUnique(x => x.Value));

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
					=> await That(subject).AreAllUnique(x => x.Value).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject =
					[new MyStringClass("a"), new MyStringClass("b"), new MyStringClass("c"),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasing_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject = [new MyStringClass("a"), new MyStringClass("A"),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject = [new MyStringClass("a"), new MyStringClass("A"),];

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value ignoring case,
					             but it contained 1 duplicate:
					               "A"

					             Collection:
					             [
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
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
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
					             but it contained 1 duplicate:
					               "a"

					             Collection:
					             [
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
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
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
					             but it contained 2 duplicates:
					               "a",
					               "b"

					             Collection:
					             [
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.AreAllUnique.ImmutableArrayStringMemberTests.MyStringClass {
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
#endif
