using System.Collections;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class UntypedTests
		{
			[Test]
			public async Task StringSubject_ShouldBindToTheStringOverload()
			{
				string subject = "abc";

				async Task Act()
					=> await (StringEqualityTypeResult<string?, IThat<string?>>)That(subject).IsNotEqualTo("abd");

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the string overload at compile time");
			}

			[Test]
			public async Task TypedArray_ShouldBindToTheTypedOverload()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				int[] unexpected = [1, 3,];

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable?, IThat<IEnumerable?>, int>)
						That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("an IEnumerable<int> expectation must keep its item type instead of falling back to object");
			}

			[Test]
			public async Task UntypedUnexpected_ShouldBindToTheUntypedOverload()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					3,
				};

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable?, IThat<IEnumerable?>, object?>)
						That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the untyped collection overload at compile time");
			}

			[Test]
			public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
			{
				IEnumerable? subject = null;
				IEnumerable? unexpected = null;

				async Task Act()
					=> await That(subject)!.IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IEnumerable? subject = null;
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject)!.IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDifferentLength_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
					3,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDifferentOrder_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					3,
					2,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDifferentOrder_WhenInAnyOrder_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					3,
					2,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
					             but it was

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
			public async Task WithDuplicates_WhenIgnoringDuplicates_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					1,
					2,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

					             Collection:
					             [1, 1, 2]

					             Expected:
					             [
					               1,
					               2
					             ]
					             """);
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentContent_ShouldSucceed()
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
				IEnumerable unexpected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 5,
					},
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentShape_ShouldSucceed()
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
				IEnumerable unexpected = new[,]
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
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a multi-dimensional array is only equal to an array with the same length in every dimension");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithDifferentShape_WhenInAnyOrder_ShouldSucceed()
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
				IEnumerable unexpected = new[,]
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
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("the options only apply to the items, the dimensions have to be the same with every option");
			}

			[Test]
			public async Task WithMultiDimensionalArrayWithSameContent_ShouldFail()
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
				IEnumerable unexpected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to collection unexpected in order,
					              but it was

					              Collection:
					              {Formatter.Format(subject)}

					              Expected:
					              {Formatter.Format(unexpected)}
					              """)
					.Because("both arrays are listed as the formatter writes a multi-dimensional array");
			}

			[Test]
			public async Task WithMultiDimensionalSubject_WhenUnexpectedIsATypedArrayWithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new[,]
				{
					{
						1, 2, 3, 4,
					},
				};
				int[] unexpected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a multi-dimensional array is not equal to a one-dimensional array with the same items");
			}

			[Test]
			public async Task WithMultiDimensionalUnexpected_WhenSubjectIsOneDimensionalWithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
					3,
					4,
				};
				IEnumerable unexpected = new[,]
				{
					{
						1, 2,
					},
					{
						3, 4,
					},
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a one-dimensional collection is not equal to a multi-dimensional array with the same items");
			}

			[Test]
			public async Task WithSameItems_ShouldFail()
			{
				IEnumerable subject = new ArrayList
				{
					1,
					2,
				};
				IEnumerable unexpected = new ArrayList
				{
					1,
					2,
				};

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was

					             Collection:
					             [1, 2]

					             Expected:
					             [
					               1,
					               2
					             ]
					             """);
			}
		}
	}
}
