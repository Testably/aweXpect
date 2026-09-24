using System.Collections;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class UntypedTests
		{
			[Fact]
			public async Task StringSubject_ShouldBindToTheStringOverload()
			{
				string subject = "abc";

				async Task Act()
					=> await (StringEqualityTypeResult<string?, IThat<string?>>)That(subject).IsEqualTo("abc");

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the string overload at compile time");
			}

			[Fact]
			public async Task StringSubject_WithDifferentValue_ShouldFail()
			{
				string subject = "abc";

				async Task Act()
					=> await That(subject).IsEqualTo("abd");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "abd",
					             but it was "abc" which differs at index 2:
					                  ↓ (actual)
					               "abc"
					               "abd"
					                  ↑ (expected)
					             """);
			}

			[Fact]
			public async Task TypedArray_ShouldBindToTheTypedOverload()
			{
				IEnumerable subject = new ArrayList { 1, 2, };
				int[] expected = [1, 2,];

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("an IEnumerable<int> expectation must keep its item type instead of falling back to object");
			}

			[Fact]
			public async Task UntypedExpected_ShouldBindToTheUntypedOverload()
			{
				IEnumerable subject = new ArrayList { 1, 2, };
				IEnumerable expected = new ArrayList { 1, 2, };

				async Task Act()
					=> await (ObjectCollectionMatchResult<IEnumerable, IThat<IEnumerable?>, object?>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the declared result type pins the untyped collection overload at compile time");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IEnumerable subject = new ArrayList { 1, 2, };
				IEnumerable? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but the expected collection was <null>

					             Collection:
					             [1, 2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				IEnumerable? subject = null;
				IEnumerable? expected = null;

				async Task Act()
					=> await That(subject)!.IsEqualTo(expected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;
				IEnumerable expected = new ArrayList { 1, 2, };

				async Task Act()
					=> await That(subject)!.IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithDifferentLength_ShouldFail()
			{
				IEnumerable subject = new ArrayList { 1, 2, 3, };
				IEnumerable expected = new ArrayList { 1, 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDifferentOrder_ShouldFail()
			{
				IEnumerable subject = new ArrayList { 1, 3, 2, };
				IEnumerable expected = new ArrayList { 1, 2, 3, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDifferentOrder_WhenInAnyOrder_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList { 1, 3, 2, };
				IEnumerable expected = new ArrayList { 1, 2, 3, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("the untyped overload must support the same match options as the typed overloads");
			}

			[Fact]
			public async Task WithDuplicates_WhenIgnoringDuplicates_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList { 1, 1, 2, };
				IEnumerable expected = new ArrayList { 1, 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("the untyped overload must support the same match options as the typed overloads");
			}

			[Fact]
			public async Task WithLazySubject_ShouldOnlyEnumerateOnce()
			{
				int enumerations = 0;
				IEnumerable subject = LazyItems();
				IEnumerable expected = new ArrayList { 1, 2, 3, };

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

			[Fact]
			public async Task WithMultiDimensionalArrayWithDifferentContent_ShouldFail()
			{
				IEnumerable subject = new[,] { { 1, 2, }, { 3, 4, }, };
				IEnumerable expected = new[,] { { 1, 2, }, { 3, 5, }, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it contained item 4 at index 3 instead of 5

					             Collection:
					             [1, 2, 3, 4]

					             Expected:
					             [
					               1,
					               2,
					               3,
					               5
					             ]
					             """);
			}

			[Fact]
			public async Task WithMultiDimensionalArrayWithDifferentShape_ShouldSucceed()
			{
				IEnumerable subject = new[,] { { 1, 2, 3, }, { 4, 5, 6, }, };
				IEnumerable expected = new[,] { { 1, 2, }, { 3, 4, }, { 5, 6, }, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because(
						"a multi-dimensional array has no shape as an IEnumerable, so it is compared by its flattened content");
			}

			[Fact]
			public async Task WithMultiDimensionalArrayWithSameContent_ShouldSucceed()
			{
				IEnumerable subject = new[,] { { 1, 2, }, { 3, 4, }, };
				IEnumerable expected = new[,] { { 1, 2, }, { 3, 4, }, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the items are compared, although the arrays are different instances");
			}

			[Fact]
			public async Task WithObjectArrayWithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList { 1, 2, };
				object[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameItems_ShouldSucceed()
			{
				IEnumerable subject = new ArrayList { 1, 2, };
				IEnumerable expected = new ArrayList { 1, 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the items are compared, although the collections are different instances");
			}
		}
	}
}
