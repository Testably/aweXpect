using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed class CombinedMultiLineResultTests
{
	public sealed class ObjectTests
	{
		[Test]
		public async Task IsEqualTo_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};
			MyClass expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenEquivalent_ShouldIndentTheDifferencesLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};
			MyClass expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected).Equivalent());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equivalent to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it was not:
				                   Property Value differed:
				                       Actual: 1
				                     Expected: 2

				             [01] Equivalency options:
				              - include public fields and properties
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenInAnyCombination_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};
			MyClass expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAny(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that subject is equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenNested_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};
			MyClass expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(
					That(true).IsFalse(),
					ThatAll(That(subject).IsEqualTo(expected)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that true is False
				               Expected all of the following to succeed:
				                [02] Expected that subject is equal to CombinedMultiLineResultTests.MyClass {
				                       Value = 2
				                     }
				             but
				              [01] it was True
				                [02] it was CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenNullableStruct_ShouldIndentTheResultLikeTheEntry()
		{
			MyStruct? subject = new MyStruct
			{
				Value = 1,
			};
			MyStruct expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to CombinedMultiLineResultTests.MyStruct {
				                     Value = 2
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyStruct {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenStruct_ShouldIndentTheResultLikeTheEntry()
		{
			MyStruct subject = new()
			{
				Value = 1,
			};
			MyStruct expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to CombinedMultiLineResultTests.MyStruct {
				                     Value = 2
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyStruct {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsNotEqualTo_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsNotEqualTo(subject));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is not equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsNotEqualTo_WhenEquivalent_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsNotEqualTo(subject).Equivalent());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is not equivalent to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }, which is considered equivalent

				             [01] Equivalency options:
				              - include public fields and properties
				             """);
		}

		[Test]
		public async Task IsNotOneOf_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsNotOneOf(subject));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is not one of [CombinedMultiLineResultTests.MyClass { Value = 1 }]
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """);
		}

		[Test]
		public async Task IsOneOf_WhenSingleCandidate_ShouldIndentTheResultLikeTheEntry()
		{
			MyClass subject = new()
			{
				Value = 1,
			};
			MyClass expected = new()
			{
				Value = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsOneOf(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is one of [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it was CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """);
		}
	}

	public sealed class CollectionTests
	{
		[Test]
		public async Task ContainsKeys_ShouldIndentTheMissingKeysLikeTheEntry()
		{
			Dictionary<int, int> subject = new()
			{
				[1] = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).ContainsKeys(3, 4));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains keys [3, 4]
				             but
				              [01] it did not contain [
				                     3,
				                     4
				                   ]

				             [01] Dictionary:
				             {[1] = 2}
				             """);
		}

		[Test]
		public async Task ContainsValues_ShouldIndentTheMissingValuesLikeTheEntry()
		{
			Dictionary<int, int> subject = new()
			{
				[1] = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).ContainsValues(3, 4));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains values [3, 4]
				             but
				              [01] it did not contain [
				                     3,
				                     4
				                   ]

				             [01] Dictionary:
				             {[1] = 2}
				             """);
		}

		[Test]
		public async Task DoesNotContainKeys_ShouldIndentTheExistingKeysLikeTheEntry()
		{
			Dictionary<int, int> subject = new()
			{
				[1] = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContainKeys(1, 3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain keys [1, 3]
				             but
				              [01] it contained [
				                     1
				                   ]

				             [01] Dictionary:
				             {[1] = 2}
				             """);
		}

		[Test]
		public async Task DoesNotContainValues_ShouldIndentTheExistingValuesLikeTheEntry()
		{
			Dictionary<int, int> subject = new()
			{
				[1] = 2,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContainValues(2, 3));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain values [2, 3]
				             but
				              [01] it contained [
				                     2
				                   ]

				             [01] Dictionary:
				             {[1] = 2}
				             """);
		}

		[Test]
		public async Task EndsWith_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await ThatAll(That(subject).EndsWith(-1, 0, 1, 2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject ends with [-1, 0, 1, 2]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     -1,
				                     0
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}

		[Test]
		public async Task EndsWith_WhenUntyped_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			IEnumerable subject = new ArrayList
			{
				1,
				2,
			};

			async Task Act()
				=> await ThatAll(That(subject).EndsWith(-1, 0, 1, 2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject ends with [-1, 0, 1, 2]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     -1,
				                     0
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}

		[Test]
		public async Task IsContainedIn_ShouldIndentTheDeviationsLikeTheEntry()
		{
			int[] subject = [1, 2,];
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(That(subject).IsContainedIn(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is contained in collection expected in order and contiguous
				             but
				              [01] it
				                     contained item 1 at index 0 that was not expected and
				                     contained item 2 at index 1 that was not expected

				             [01] Collection:
				             [1, 2]

				             [01] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task IsEmpty_ShouldIndentTheItemsLikeTheEntry()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await ThatAll(That(subject).IsEmpty());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is empty
				             but
				              [01] it was [
				                     1,
				                     2
				                   ]
				             """);
		}

		[Test]
		public async Task IsEqualTo_ShouldIndentTheDeviationsLikeTheEntry()
		{
			int[] subject = [1, 2,];
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to collection expected in order
				             but
				              [01] it
				                     contained item 1 at index 0 instead of 3 and
				                     contained item 2 at index 1 instead of 4

				             [01] Collection:
				             [1, 2]

				             [01] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenDictionaryKeys_ShouldIndentTheDeviationsLikeTheEntry()
		{
			Dictionary<int, int> subject = new()
			{
				[1] = 0,
				[2] = 0,
			};
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(That(subject).Keys.IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has keys that are equal to collection expected in order
				             but
				              [01] it
				                     contained item 1 at index 0 instead of 3 and
				                     contained item 2 at index 1 instead of 4

				             [01] Collection (keys):
				             [1, 2]

				             [01] Expected (keys):
				             [3, 4]
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenInAnyCombination_ShouldIndentTheDeviationsLikeTheEntry()
		{
			int[] subject = [1, 2,];
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAny(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected any of the following to succeed:
				              [01] Expected that subject is equal to collection expected in order
				             but
				              [01] it
				                     contained item 1 at index 0 instead of 3 and
				                     contained item 2 at index 1 instead of 4

				             [01] Collection:
				             [1, 2]

				             [01] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenNested_ShouldIndentTheDeviationsLikeTheEntry()
		{
			int[] subject = [1, 2,];
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(
					That(true).IsFalse(),
					ThatAll(That(subject).IsEqualTo(expected)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that true is False
				               Expected all of the following to succeed:
				                [02] Expected that subject is equal to collection expected in order
				             but
				              [01] it was True
				                [02] it
				                       contained item 1 at index 0 instead of 3 and
				                       contained item 2 at index 1 instead of 4

				             [02] Collection:
				             [1, 2]

				             [02] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenUntyped_ShouldIndentTheDeviationsLikeTheEntry()
		{
			IEnumerable subject = new ArrayList
			{
				1,
				2,
			};
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to collection expected in order
				             but
				              [01] it
				                     contained item 1 at index 0 instead of 3 and
				                     contained item 2 at index 1 instead of 4

				             [01] Collection:
				             [1, 2]

				             [01] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task StartsWith_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			int[] subject = [1, 2,];

			async Task Act()
				=> await ThatAll(That(subject).StartsWith(1, 2, 3, 4));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject starts with [1, 2, 3, 4]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     3,
				                     4
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}

		[Test]
		public async Task StartsWith_WhenUntyped_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			IEnumerable subject = new ArrayList
			{
				1,
				2,
			};

			async Task Act()
				=> await ThatAll(That(subject).StartsWith(1, 2, 3, 4));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject starts with [1, 2, 3, 4]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     3,
				                     4
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}
	}

#if NET8_0_OR_GREATER
	public sealed class AsyncCollectionTests
	{
		[Test]
		public async Task EndsWith_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await ThatAll(That(subject).EndsWith(-1, 0, 1, 2));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject ends with [-1, 0, 1, 2]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     -1,
				                     0
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}

		[Test]
		public async Task IsEmpty_ShouldIndentTheItemsLikeTheEntry()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await ThatAll(That(subject).IsEmpty());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is empty
				             but
				              [01] it was [
				                     1,
				                     (… and maybe more)
				                   ]
				             """);
		}

		[Test]
		public async Task IsEqualTo_ShouldIndentTheDeviationsLikeTheEntry()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);
			int[] expected = [3, 4,];

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to collection expected in order
				             but
				              [01] it
				                     contained item 1 at index 0 instead of 3 and
				                     contained item 2 at index 1 instead of 4

				             [01] Collection:
				             [1, 2]

				             [01] Expected:
				             [3, 4]
				             """);
		}

		[Test]
		public async Task StartsWith_ShouldIndentTheLackedItemsLikeTheEntry()
		{
			IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2,]);

			async Task Act()
				=> await ThatAll(That(subject).StartsWith(1, 2, 3, 4));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject starts with [1, 2, 3, 4]
				             but
				              [01] it contained only 2 items and lacked 2 items: [
				                     3,
				                     4
				                   ]

				             [01] Collection:
				             [1, 2]
				             """);
		}
	}
#endif

	private sealed class MyClass
	{
		public int Value { get; set; }
	}

	private struct MyStruct
	{
		public int Value { get; set; }
	}
}
