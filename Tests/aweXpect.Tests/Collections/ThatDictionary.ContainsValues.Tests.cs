using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class ContainsValues
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllValuesExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, 41);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllValuesExists_WithNull_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [null, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllValuesOfAnEnumerableExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> expected = new List<int> { 42, 41, };

				async Task Act()
					=> await That(subject).ContainsValues(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				int[]? expected = null;

				async Task Act()
					=> await That(subject).ContainsValues(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedThrows_ShouldThrowTheExceptionOfTheExpectedItems()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				IEnumerable<int> GetExpected()
				{
					yield return 41;
					throw new InvalidOperationException("the expected values are broken");
				}

				async Task Act()
					=> await That(subject).ContainsValues(GetExpected());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("the expected values are broken")
					.Because("an exception of the expected values is not wrapped as if the subject threw it");
			}

			[Test]
			public async Task WhenOneValueIsMissing_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values [42, 2],
					             but it did not contain [
					               2
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenOneValueIsMissing_WithNull_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValues(42, null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values [42, <null>],
					             but it did not contain [
					               <null>
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenOneValueOfAnEnumerableIsMissing_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> expected = new List<int> { 42, 2, };

				async Task Act()
					=> await That(subject).ContainsValues(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values expected,
					             but it did not contain [
					               2
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenStringValuesDifferOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).ContainsValues("BAR", "FOO").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.ContainsValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, [2] = value.AddMinutes(1), };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).ContainsValues(value.AddMilliseconds(500), value.AddMinutes(1));
				}

				await That(Act).DoesNotThrow()
					.Because("the values fall back to the default tolerance, as the items of a collection do");
			}

			[Test]
			public async Task WhenValuesAreEquivalent_WithEquivalent_ShouldSucceed()
			{
				Dictionary<int, MyClass> subject = new() { [1] = new MyClass(1), [2] = new MyClass(2), };

				async Task Act()
					=> await That(subject).ContainsValues(new MyClass(2), new MyClass(1)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValuesAreNumbersOfADifferentType_ShouldSucceed()
			{
				Dictionary<string, object> subject = new() { ["a"] = 1, ["b"] = 2.5, };

				async Task Act()
					=> await That(subject).ContainsValues(1L, 2.5m);

				await That(Act).DoesNotThrow()
					.Because("values are compared with the same object equality as the items of a collection");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllValuesExist_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsValues(41, 42));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain all values [41, 42],
					             but it contained [
					               41,
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenOneValueIsMissingAndOneExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.ContainsValues(2, 42));

				await That(Act).DoesNotThrow()
					.Because("the negation of ContainsValues only fails when all values are contained");
			}
		}

		public sealed class WithinTests
		{
			[Test]
			public async Task WhenACollectionOfNonNullableValuesLiesWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double?> subject = new() { ["a"] = 1.05, ["b"] = null, ["c"] = 2.05, };
				IEnumerable<double> expected = [1.0, 2.0,];

				async Task Act()
					=> await That(subject).ContainsValues(expected).Within(0.1);

				await That(Act).DoesNotThrow()
					.Because("non-nullable expected values are compared with the nullable values of the dictionary");
			}

			[Test]
			public async Task WhenOneValueLiesOutsideTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, ["b"] = 2.2, };

				async Task Act()
					=> await That(subject).ContainsValues(1.0, 2.0).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values [1.0, 2.0] ± 0.1,
					             but it did not contain [
					               2.0
					             ]

					             Dictionary:
					             {["a"] = 1.05, ["b"] = 2.2}
					             """);
			}

			[Test]
			public async Task WhenTheValuesLieWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, ["b"] = 2.05, };

				async Task Act()
					=> await That(subject).ContainsValues(1.0, 2.0).Within(0.1);

				await That(Act).DoesNotThrow()
					.Because("the values have the same tolerance as the items of a collection");
			}
		}

		public sealed class OverloadTests
		{
			[Test]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).ContainsValues(new List<int> { 1, });

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).ContainsValues(1);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration) { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).ContainsValues(1, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains values [1, 2],
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}
		}
	}
}
