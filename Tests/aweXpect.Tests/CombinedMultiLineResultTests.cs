using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Signaling;

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

	public sealed class ItemValueTests
	{
		[Test]
		public async Task AreEqualTo_ShouldIndentTheExpectedItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).All().AreEqualTo(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } for all items
				             but
				              [01] only 1 of 2 were
				             """).AsPrefix();
		}

		[Test]
		public async Task Contains_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).Contains(a).Exactly(2.Times()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } exactly twice
				             but
				              [01] it contained CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } once
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotContain_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContain(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it contained CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at least once
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotEndWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotEndWith(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not end with [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it did end with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 2
				                     }
				                   ]
				             """);
		}

		[Test]
		public async Task DoesNotEndWith_WhenUntyped_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IEnumerable subject = new ArrayList
			{
				a,
				b,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotEndWith(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not end with [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it did end with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 2
				                     }
				                   ]
				             """);
		}

		[Test]
		public async Task DoesNotHaveItem_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotHaveItem(a).AtIndex(0));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not have an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             but
				              [01] it had item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotHaveItemThat_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotHaveItemThat(x => x.IsSameAs(a)).AtIndex(0));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not have an item that refers to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             but
				              [01] it had item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotHaveSingle_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass[] subject = [a,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotComplyWith(x => x.HasSingle()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not have a single item
				             but
				              [01] it had the single item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotStartWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).DoesNotStartWith(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not start with [CombinedMultiLineResultTests.MyClass { Value = 1 }]
				             but
				              [01] it did start with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				                   ]
				             """);
		}

		[Test]
		public async Task DoesNotStartWith_WhenUntyped_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IEnumerable subject = new ArrayList
			{
				a,
				b,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotStartWith(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not start with [CombinedMultiLineResultTests.MyClass { Value = 1 }]
				             but
				              [01] it did start with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				                   ]
				             """);
		}

		[Test]
		public async Task EndsWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).EndsWith(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject ends with [CombinedMultiLineResultTests.MyClass { Value = 1 }]
				             but
				              [01] it contained item CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } at index 1 instead of CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task HasItem_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).HasItem(b).AtIndex(0));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } at index 0
				             but
				              [01] it had item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             """).AsPrefix();
		}

		[Test]
		public async Task HasItemThat_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).HasItemThat(x => x.IsSameAs(b)).AtIndex(0));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has an item that refers to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } at index 0
				             but
				              [01] it had item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             """).AsPrefix();
		}

		[Test]
		public async Task IsInAscendingOrder_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [b, a,];

			async Task Act()
				=> await ThatAll(That(subject).IsInAscendingOrder().Using(new MyClassComparer()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is in ascending order using CombinedMultiLineResultTests.MyClassComparer
				             but
				              [01] it had CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } before CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }, which is not in ascending order
				             """).AsPrefix();
		}

		[Test]
		public async Task StartsWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			MyClass[] subject = [a, b,];

			async Task Act()
				=> await ThatAll(That(subject).StartsWith(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject starts with [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it contained item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0 instead of CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task StartsWith_WhenUntyped_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IEnumerable subject = new ArrayList
			{
				a,
				b,
			};

			async Task Act()
				=> await ThatAll(That(subject).StartsWith(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject starts with [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it contained item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0 instead of CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}
	}

	public sealed class DictionaryValueTests
	{
		[Test]
		public async Task Contains_ShouldIndentTheEntryLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).Contains(new KeyValuePair<MyClass, MyClass>(a, b)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains [CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }] = CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it contained key CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } with value CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task Contains_WhenKeyIsMissing_ShouldIndentTheKeyLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).Contains(new KeyValuePair<MyClass, MyClass>(b, b)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains [CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }] = CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it did not contain key CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task ContainsKey_ShouldIndentTheKeyLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).ContainsKey(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains key CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it did not contain key CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task ContainsValue_ShouldIndentTheValueLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).ContainsValue(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains value CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             but
				              [01] it did not contain value CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotContain_ShouldIndentTheEntryLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContain(new KeyValuePair<MyClass, MyClass>(a, a)));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain [CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }] = CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it did
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotContainKey_ShouldIndentTheKeyLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContainKey(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain key CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it did
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotContainValue_ShouldIndentTheValueLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};

			async Task Act()
				=> await ThatAll(That(subject).DoesNotContainValue(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not contain value CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   }
				             but
				              [01] it did
				             """).AsPrefix();
		}

		[Test]
		public async Task IsEqualTo_ShouldIndentTheKeysAndValuesLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			Dictionary<MyClass, MyClass> subject = new()
			{
				[a] = a,
			};
			Dictionary<MyClass, MyClass> expected = new()
			{
				[a] = b,
				[b] = b,
			};

			async Task Act()
				=> await ThatAll(That(subject).IsEqualTo(expected));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject is equal to dictionary expected
				             but
				              [01] it lacked key CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } and contained key CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } with value CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } instead of CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   }
				             """).AsPrefix();
		}
	}

	public sealed class SignalerTests
	{
		[Test]
		public async Task DidNotSignal_ShouldIndentTheParametersLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			Signaler<MyClass> subject = new();
			subject.Signal(a);

			async Task Act()
				=> await ThatAll(That(subject).DidNotSignal().Within(10.Milliseconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has never recorded the callback within 0:00.010
				             but
				              [01] it was recorded once in [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				                   ] after *
				             """).AsWildcard();
		}

		[Test]
		public async Task Signaled_ShouldIndentTheParametersLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			Signaler<MyClass> subject = new();
			subject.Signal(a);

			async Task Act()
				=> await ThatAll(That(subject).Signaled().With(x => x.Value == 5).Within(10.Milliseconds()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has recorded the callback at least once with x => x.Value == 5 within 0:00.010
				             but
				              [01] it was never recorded in [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				                   ] within *
				             """).AsWildcard();
		}
	}

#if NET8_0_OR_GREATER
	public sealed class AsyncCollectionTests
	{
		[Test]
		public async Task Contains_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IAsyncEnumerable<MyClass> subject = ThatAsyncEnumerable.ToAsyncEnumerable(a, b);

			async Task Act()
				=> await ThatAll(That(subject).Contains(a).Exactly(2.Times()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject contains an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } exactly twice
				             but
				              [01] it contained CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } once
				             """).AsPrefix();
		}

		[Test]
		public async Task DoesNotEndWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IAsyncEnumerable<MyClass> subject = ThatAsyncEnumerable.ToAsyncEnumerable(a, b);

			async Task Act()
				=> await ThatAll(That(subject).DoesNotEndWith(b));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not end with [CombinedMultiLineResultTests.MyClass { Value = 2 }]
				             but
				              [01] it did end with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 2
				                     }
				                   ]
				             """);
		}

		[Test]
		public async Task DoesNotStartWith_ShouldIndentTheItemsLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IAsyncEnumerable<MyClass> subject = ThatAsyncEnumerable.ToAsyncEnumerable(a, b);

			async Task Act()
				=> await ThatAll(That(subject).DoesNotStartWith(a));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject does not start with [CombinedMultiLineResultTests.MyClass { Value = 1 }]
				             but
				              [01] it did start with [
				                     CombinedMultiLineResultTests.MyClass {
				                       Value = 1
				                     }
				                   ]
				             """);
		}

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
		public async Task HasItem_ShouldIndentTheItemLikeTheEntry()
		{
			MyClass a = new()
			{
				Value = 1,
			};
			MyClass b = new()
			{
				Value = 2,
			};
			IAsyncEnumerable<MyClass> subject = ThatAsyncEnumerable.ToAsyncEnumerable(a, b);

			async Task Act()
				=> await ThatAll(That(subject).HasItem(b).AtIndex(0));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected all of the following to succeed:
				              [01] Expected that subject has an item equal to CombinedMultiLineResultTests.MyClass {
				                     Value = 2
				                   } at index 0
				             but
				              [01] it had item CombinedMultiLineResultTests.MyClass {
				                     Value = 1
				                   } at index 0
				             """).AsPrefix();
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

	private sealed class MyClassComparer : IComparer<MyClass>
	{
		public int Compare(MyClass? x, MyClass? y) => x!.Value.CompareTo(y!.Value);
	}

	private struct MyStruct
	{
		public int Value { get; set; }
	}
}
