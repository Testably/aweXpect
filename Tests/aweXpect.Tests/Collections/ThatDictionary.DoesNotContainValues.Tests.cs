using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContainValues
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllValuesDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(0, 2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllValuesDoNotExist_WithNull_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAtLeastOneValueExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(42, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [42, 2],
					             but it contained [
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenAtLeastOneValueExists_WithNull_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [null, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [2, <null>],
					             but it contained [
					               <null>
					             ]

					             Dictionary:
					             {[1] = <null>, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenAtLeastOneValueIsANumberOfADifferentType_ShouldFail()
			{
				Dictionary<string, object> subject = new()
				{
					["a"] = 1,
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, 1L);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [2, 1],
					             but it contained [
					               1
					             ]

					             Dictionary:
					             {
					               ["a"] = 1
					             }
					             """);
			}

			[Test]
			public async Task WhenOneStringValueDiffersOnlyInCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string?> subject = new()
				{
					[1] = "foo",
					[2] = "bar",
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues("BAZ", "BAR").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values ["BAZ", "BAR"] ignoring case,
					             but it contained [
					               "BAR"
					             ]

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar"
					             }
					             """);
			}

			[Test]
			public async Task WhenOneValueOfAnEnumerableExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> unexpected = new List<int>
				{
					42,
					2,
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values unexpected,
					             but it contained [
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotContainValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new()
				{
					[1] = value,
				};
				DateTime unexpected = value.AddMilliseconds(500);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).DoesNotContainValues(value.AddSeconds(5), unexpected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain values [{Formatter.Format(value.AddSeconds(5))}, {Formatter.Format(unexpected)}] ± 0:01,
					              but it contained [
					                {Formatter.Format(unexpected)}
					              ]

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the values fall back to the default tolerance, as the items of a collection do");
			}

			[Test]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenValuesOfAnEnumerableDoNotExist_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				IEnumerable<int> unexpected = new List<int>
				{
					0,
					2,
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoValueExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2,], [41, 42,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainValues(3, 4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains any of values [3, 4],
					             but it did not contain [
					               3,
					               4
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42}
					             """)
					.Because("the negation of DoesNotContainValues only fails when none of the values is contained");
			}

			[Test]
			public async Task WhenOneValueIsMissingAndOneExists_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2,], [41, 42,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainValues(3, 42));

				await That(Act).DoesNotThrow()
					.Because("the negation of DoesNotContainValues succeeds when any of the values is contained");
			}
		}

		public sealed class WithinTests
		{
			[Test]
			public async Task WhenNoValueLiesWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new()
				{
					["a"] = 1.2,
					["b"] = 2.2,
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues(1.0, 2.0).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneValueLiesWithinTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new()
				{
					["a"] = 1.2,
					["b"] = 2.05,
				};

				async Task Act()
					=> await That(subject).DoesNotContainValues(1.0, 2.0).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [1.0, 2.0] ± 0.1,
					             but it contained [
					               2.0
					             ]

					             Dictionary:
					             {["a"] = 1.2, ["b"] = 2.05}
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Test]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				Dictionary<string, int> result = await That(subject).DoesNotContainValues(new List<int>
				{
					2,
				});

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new()
				{
					{
						"a", 1
					},
				};

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).DoesNotContainValues(2);

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
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration)
					{
						["a"] = 1,
						["b"] = 2,
					};

				async Task Act()
					=> await That(subject).DoesNotContainValues(7, 8);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [7, 8],
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}
		}
	}
}
