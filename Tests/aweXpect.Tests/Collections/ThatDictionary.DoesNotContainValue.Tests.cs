using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContainValue
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNullValueDoesNotExist_ShouldSucceed()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullValueExists_ShouldFail()
			{
				IDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, null, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value <null>,
					             but it did

					             Dictionary:
					             {[1] = 41, [2] = <null>, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenStringValueDiffersInMoreThanCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).DoesNotContainValue("BAZ").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringValueDiffersOnlyInCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string?> subject = new() { [1] = "foo", [2] = "bar", };

				async Task Act()
					=> await That(subject).DoesNotContainValue("BAR").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value "BAR" ignoring case,
					             but it did

					             Dictionary:
					             {
					               [1] = "foo",
					               [2] = "bar"
					             }
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotContainValue("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				DateTime unexpected = value.AddMilliseconds(500);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).DoesNotContainValue(unexpected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not contain value {Formatter.Format(unexpected)} ± 0:01,
					              but it did

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the values fall back to the default tolerance, as the items of a collection do");
			}

			[Fact]
			public async Task WhenValueExists_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(42);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 42,
					             but it did

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenValueIsANumberOfADifferentType_ShouldFail()
			{
				Dictionary<string, object> subject = new() { ["a"] = 1, };

				async Task Act()
					=> await That(subject).DoesNotContainValue(1L);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 1,
					             but it did

					             Dictionary:
					             {
					               ["a"] = 1
					             }
					             """);
			}

			[Fact]
			public async Task WhenValueIsMissing_ShouldSucceed()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueMatchesTheComparer_ShouldFail()
			{
				IDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValue(2).Using(new AllEqualComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 2 using AllEqualComparer,
					             but it did

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenTheValueLiesOutsideTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.2, };

				async Task Act()
					=> await That(subject).DoesNotContainValue(1.0).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheValueLiesWithinTheTolerance_ShouldFail()
			{
				Dictionary<string, TimeSpan> subject = new() { ["a"] = 61.Seconds(), };

				async Task Act()
					=> await That(subject).DoesNotContainValue(1.Minutes()).Within(1.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain value 1:00 ± 0:01,
					             but it did

					             Dictionary:
					             {
					               ["a"] = 1:01
					             }
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).DoesNotContainValue(2);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).DoesNotContainValue(2);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
