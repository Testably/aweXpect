#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

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
				IEnumerable<int> subject = ToEnumerable([1, 1, 1,]);

				async Task Act()
					=> await That(subject).AreAllUnique().Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItContainsDuplicates_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

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
				IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1, 2, -1,]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it was <null>
					             """);
			}
		}

		public sealed class ImmutableArrayNegatedTests
		{
			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

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
				IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).AreAllUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items,
					             but it was <null>
					             """);
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 1, 1,]).Select(x => new MyClass(x));

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,]).Select(x => new MyClass(x));

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItContainsDuplicates_ShouldFail()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3, 1,]).Select(x => new MyClass(x));

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
				IEnumerable<MyClass> subject =
					ToEnumerable([1, 2, 3, 1, 2, -1,]).Select(x => new MyClass(x));

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MyClass>? subject = null;

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
					             but it was <null>
					             """);
			}
		}

		public sealed class ImmutableArrayNegatedMemberTests
		{
			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldFail()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

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
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3, 1,], x => new MyClass(x));

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
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "a", "a",]).Select(x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllItemsAreUnique_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c",]).Select(x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasing_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",]).Select(x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",]).Select(x => new MyStringClass(x));

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
				IEnumerable<MyStringClass> subject =
					ToEnumerable(["a", "b", "c", "a",]).Select(x => new MyStringClass(x));

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
				IEnumerable<MyStringClass> subject =
					ToEnumerable(["a", "b", "c", "a", "b", "x",]).Select(x => new MyStringClass(x));

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MyStringClass>? subject = null;

				async Task Act()
					=> await That(subject).AreAllUnique(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             only has unique items for x => x.Value,
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
#endif
