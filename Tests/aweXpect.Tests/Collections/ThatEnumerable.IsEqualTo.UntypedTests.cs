using System.Collections;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class UntypedTests
		{
			[Test]
			public async Task StringSubject_ShouldBindToTheStringOverload()
			{
				string subject = "abc";

				async Task Act()
					=> await (StringEqualityTypeResult<string?, IThat<string?>>)That(subject).IsEqualTo("abc");

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the string overload at compile time");
			}

			[Test]
			public async Task StringSubject_WithDifferentValue_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsEqualTo("abd");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "abd",
					             but it was "abc", which differs at index 2:
					                  ↓ (actual)
					               "abc"
					               "abd"
					                  ↑ (expected)
					             """);
			}

			[Test]
			public async Task TypedArray_ShouldBindToTheTypedOverload()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				int[] expected = [1, 2,];

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable?, IThat<IEnumerable?>, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("an IEnumerable<int> expectation must keep its item type instead of falling back to object");
			}

			[Test]
			public async Task UntypedExpected_ShouldBindToTheUntypedOverload()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable?, IThat<IEnumerable?>, object?>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the untyped collection overload at compile time");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but the expected collection was <null>

					             Collection:
					             [1, 2]
					             """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				IEnumerable? subject = null;
				IEnumerable? expected = null;

				async Task Act()
					=> await That(subject)!.IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;
				IEnumerable expected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject)!.IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithDifferentLength_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
					3,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item 3 at index 2 that was not expected

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               1,
					               2
					             ]
					             """);
			}

			[Test]
			public async Task WithDifferentOrder_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					3,
					2,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 3 at index 1 instead of 2 and
					               contained item 2 at index 2 instead of 3
					             (but the items match in a different order)

					             Collection:
					             [1, 3, 2]

					             Expected:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Test]
			public async Task WithDifferentOrder_WhenInAnyOrder_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					3,
					2,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("the untyped overload must support the same match options as the typed overloads");
			}

			[Test]
			public async Task WithDuplicates_WhenIgnoringDuplicates_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					1,
					2,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("the untyped overload must support the same match options as the typed overloads");
			}

			[Test]
			public async Task WithLazySubject_ShouldOnlyEnumerateOnce()
			{
				int enumerations = 0;
				IEnumerable subject = LazyItems();
				IEnumerable expected = new ArrayList
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
				await That(enumerations).IsEqualTo(1)
					.Because("the subject must be materialized once instead of being enumerated again");

				IEnumerable LazyItems()
				{
					enumerations++;
					yield return 1;
					yield return 2;
					yield return 3;
				}
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentContent_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 5,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order,
					              but it contained item 4 at index [1,1] instead of 5

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """)
					.Because("both arrays are listed as the formatter writes a multi-dimensional array");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentContent_WhenIgnoringDuplicates_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 5,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order ignoring duplicates,
					              but it
					                contained item 4 at index [1,1] that was not expected and
					                lacked 1 of 4 expected items: 5

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentContent_WhenInAnyOrder_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						9, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						4, 3,
					},
					{
						2, 1,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in any order,
					              but it
					                contained item 9 at index [1,0] that was not expected and
					                lacked 1 of 4 expected items: 3

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentContent_WhenInAnyOrderIgnoringDuplicates_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						9, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						4, 3,
					},
					{
						2, 1,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in any order ignoring duplicates,
					              but it
					                contained item 9 at index [1,0] that was not expected and
					                lacked 1 of 4 expected items: 3

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentOrder_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2, 3,
					},
					{
						4, 5, 6,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2, 4,
					},
					{
						5, 3, 6,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order,
					              but it contained item 3 at index [0,2] in wrong order
					              (but the items match in a different order)

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayOfRankThreeWithDifferentContent_ShouldFail()
			{
				IEnumerable subject = new[,,]
				{
					{
						{
							1, 2,
						},
					},
					{
						{
							3, 4,
						},
					},
				};
				IEnumerable expected = new[,,]
				{
					{
						{
							1, 0,
						},
					},
					{
						{
							0, 4,
						},
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order,
					              but it
					                contained item 2 at index [0,0,1] instead of 0 and
					                contained item 3 at index [1,0,0] instead of 0

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentRank_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new[,,]
				{
					{
						{
							1, 2,
						},
						{
							3, 4,
						},
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had rank 2 instead of 3*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentShape_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2, 3,
					},
					{
						4, 5, 6,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
					{
						5, 6,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to collection expected in order,
					              but it had dimensions [2,3] instead of [3,2]

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(expected)}
					              """)
					.Because("a multi-dimensional array is only equal to an array with the same length in every dimension");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentShape_WhenIgnoringDuplicates_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 1, 2,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it had dimensions [1,3] instead of [1,2]*
					             """).AsWildcard()
					.Because("the options only apply to the items, the dimensions have to be the same with every option");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentShape_WhenInAnyOrder_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2, 3,
					},
					{
						4, 5, 6,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						6, 5,
					},
					{
						4, 3,
					},
					{
						2, 1,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it had dimensions [2,3] instead of [3,2]*
					             """).AsWildcard()
					.Because("the options only apply to the items, the dimensions have to be the same with every option");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithoutItemsWithDifferentShape_ShouldFail()
			{
				IEnumerable subject = new int[0, 2];
				IEnumerable expected = new int[2, 0];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had dimensions [0,2] instead of [2,0]*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithSameContent_ShouldSucceed()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the items are compared, although the arrays are different instances");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithSameContent_WhenAnEarlierAttemptHadADifferentShape_ShouldSucceed()
			{
				int calls = 0;
				Func<IEnumerable> subject = () => calls++ == 0
					? new[,]
					{
						{
							1, 2, 3, 4,
						},
					}
					: new[,]
					{
						{
							1, 2,
						},
						{
							3, 4,
						},
					};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the dimensions are compared again in every attempt");
				await That(calls).IsEqualTo(2);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithSameContentInDifferentOrder_WhenInAnyOrder_ShouldSucceed()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new[,]
				{
					{
						4, 3,
					},
					{
						2, 1,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("the dimensions are the same, and the items are matched regardless of their position");
			}

			[Test]
			public async Task WithMultiDimensionalExpected_WhenSubjectIsOneDimensionalWithSameItems_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
					3,
					4,
				};
				IEnumerable expected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had rank 1 instead of 2*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithMultiDimensionalSubject_WhenExpectedIsAnotherCollectionWithSameItems_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
					3,
					4,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had rank 2 instead of 1*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithMultiDimensionalSubject_WhenExpectedIsASingleString_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						"a",
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo("a");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection "a" in order,
					             but it had rank 2 instead of 1*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithMultiDimensionalSubject_WhenExpectedIsATypedArrayWithSameItems_ShouldFail()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2, 3, 4,
					},
				};
				int[] expected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had rank 2 instead of 1*
					             """).AsWildcard()
					.Because("a multi-dimensional array is not equal to a one-dimensional array with the same items");
			}

			[Test]
			public async Task WithMultiDimensionalSubjectOfItsOwnType_WhenShapeIsDifferent_ShouldFail()
			{
				int[,] subject =
				{
					{
						1, 2, 3, 4,
					},
				};
				int[,] expected =
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it had dimensions [1,4] instead of [2,2]*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithObjectArrayWithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				object[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable expected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the items are compared, although the collections are different instances");
			}
		}
	}
}
