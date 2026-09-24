using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class EnumerableInSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<int>() in order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]

					             Expected:
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 1 at index 0 instead of 100 and
					               contained item 2 at index 1 instead of 101 and
					               contained item 3 at index 2 instead of 102 and
					               contained item 4 at index 3 instead of 103 and
					               contained item 5 at index 4 instead of 104 and
					               contained item 6 at index 5 instead of 105 and
					               contained item 7 at index 6 instead of 106 and
					               contained item 8 at index 7 instead of 107 and
					               contained item 9 at index 8 instead of 108 and
					               contained item 10 at index 9 instead of 109 and
					               contained item 11 at index 10 instead of 110

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked all 3 expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 1 at index 0 instead of 101 and
					               contained item 2 at index 1 instead of 102 and
					               contained item 3 at index 2 instead of 103 and
					               contained item 4 at index 3 instead of 104 and
					               contained item 5 at index 4 instead of 105 and
					               contained item 6 at index 5 instead of 106 and
					               contained item 7 at index 6 instead of 107 and
					               contained item 8 at index 7 instead of 108 and
					               contained item 9 at index 8 instead of 109 and
					               contained item 10 at index 9 instead of 110
					             
					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsADifferentString_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject).IsEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection "foo" in order,
					             but it contained item "bar" at index 1 that was not expected

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]

					             Expected:
					             [
					               "foo"
					             ]
					             """)
					.Because("a string argument is a single expected item and not a sequence of characters");
			}

			[Fact]
			public async Task WhenExpectedIsAString_ShouldUseItAsSingleExpectedItem()
			{
				IEnumerable subject = ToEnumerable(["foo",]);

				async Task Act()
					=> await That(subject).IsEqualTo("foo");

				await That(Act).DoesNotThrow()
					.Because("a string argument is a single expected item and not a sequence of characters");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but the expected collection was <null>

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsTheNullLiteral_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection null in order,
					             but the expected collection was <null>

					             Collection:
					             [
					               1,
					               2
					             ]
					             """)
					.Because("a null literal binds to the string overload, but still expects no collection");
			}

			[Fact]
			public async Task WhenItemsAreOfASubtype_ShouldSucceed()
			{
				ArgumentException item1 = new("a");
				ArgumentException item2 = new("b");
				IEnumerable subject = ToEnumerable(item1, item2);
				Exception[] expected = [item1, item2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsHaveADifferentType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);
				long[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 1 (int) at index 0 instead of 1 (long) and
					               contained item 2 (int) at index 1 instead of 2 (long)

					             Collection:
					             [1, 2]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WhenItemsHaveMixedTypes_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object>(1, "a");
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item "a" at index 1 instead of 2

					             Collection:
					             [1, "a"]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WhenNullItemIsComparedToNullableValueType_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable<object?>(1, null);
				int?[] expected = [1, null,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullItemIsComparedToValueType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object?>(1, null);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item <null> at index 1 instead of 2

					             Collection:
					             [1, <null>]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldSucceed()
			{
				IEnumerable? subject = null;
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject)!.IsEqualTo(expected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectContainsTheCharactersOfTheExpectedString_ShouldFail()
			{
				IEnumerable subject = ToEnumerable('f', 'o', 'o');

				async Task Act()
					=> await That(subject).IsEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection "foo" in order,
					             but it
					               contained item 'f' at index 0 instead of "foo" and
					               contained item 'o' at index 1 that was not expected and
					               contained item 'o' at index 2 that was not expected

					             Collection:
					             ['f', 'o', 'o']

					             Expected:
					             [
					               "foo"
					             ]
					             """)
					.Because("a string argument is a single expected item and not a sequence of characters");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.IsEqualTo(Array.Empty<int>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<int>() in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               lacked 1 of 6 expected items: "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c"
					             (but the items match in a different order)

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item "c" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "b" at index 1 instead of "a" and
					               contained item "c" at index 2 instead of "b" and
					               lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "a" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c" and
					               contained item "c" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<int>()).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<int>() in order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]

					             Expected:
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked all 3 unique expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked all 2 unique expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 unique expected items
					             
					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               lacked 3 of 6 expected items:
					                 "x",
					                 "y",
					                 "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c"
					             (but the items match in a different order)

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithItemOfADifferentType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object>(1, 1, "a", 2);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "a" at index 2 instead of 2 and
					               lacked 1 of 2 expected items: 2

					             Collection:
					             [1, 1, "a", 2]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableInAnyOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<int>()).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<int>() in any order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]

					             Expected:
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked all 3 expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 expected items
					             
					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               lacked 3 of 6 expected items:
					                 "x",
					                 "y",
					                 "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item "c" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item "a" at index 1 that was not expected

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithItemOfADifferentType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object>(2, "a", 1);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it contained item "a" at index 1 that was not expected

					             Collection:
					             [2, "a", 1]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WithItemOfAnIndistinguishableType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object>(1L, 2L, 3);
				long[] expected = [1L, 2L, 3L,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item 3 (int) at index 2 that was not expected and
					               lacked 1 of 3 expected items: 3 (long)

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("the unexpected and the missing item format equally, so only their runtime types tell them apart");
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<int>()).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<int>() in any order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and maybe more)
					             ]

					             Expected:
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked all 3 unique expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable subject = Array.Empty<string>();
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked all 2 unique expected items
					             
					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 unique expected items
					             
					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               lacked 3 of 6 expected items:
					                 "x",
					                 "y",
					                 "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithItemOfADifferentType_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<object>(2, "a", 1, 2);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it contained item "a" at index 1 that was not expected

					             Collection:
					             [2, "a", 1, 2]

					             Expected:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
