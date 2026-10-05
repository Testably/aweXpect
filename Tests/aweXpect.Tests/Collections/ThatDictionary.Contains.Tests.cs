using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["b"] = 1,
					             but it did not contain key "b"

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenPairExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 2));

				await That(Act).DoesNotThrow()
					.Because("the dictionary holds the key with that value");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).Contains(1, value.AddMilliseconds(500));
				}

				await That(Act).DoesNotThrow()
					.Because("the value of the entry falls back to the default tolerance, as the items of a collection do");
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldMentionIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				DateTime expected = value.AddSeconds(2);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).Contains(1, expected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              contains [1] = {Formatter.Format(expected)} ± 0:01,
					              but it contained key 1 with value {Formatter.Format(value)}

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the applied default tolerance is part of the expectation");
			}
		}

		public sealed class ComparerTests
		{
			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 1));

				await That(Act).DoesNotThrow()
					.Because("ContainsKey finds the key through the comparer, and Contains must not contradict it");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["A"] = 2,
					             but it contained key "A" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains("b", 2);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains("a", 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsNull_ShouldThrowArgumentNullException()
			{
				Dictionary<string, int> subject = new()
				{
					["a"] = 1,
				};

				async Task Act()
					=> await That(subject).Contains(null!, 1);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix()
					.Because("a null key can never be contained, like for ContainsKey");
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenThePairDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<int, string?>(1, "LET IT BE")).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheValueDiffersInMoreThanCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };

				async Task Act()
					=> await That(subject).Contains(1, "Yesterday").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains [1] = "Yesterday" ignoring case,
					             but it contained key 1 with value "Let It Be"

					             Dictionary:
					             {
					               [1] = "Let It Be"
					             }
					             """);
			}

			[Fact]
			public async Task WhenTheValueDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };

				async Task Act()
					=> await That(subject).Contains(1, "let it be").IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("the value of an entry has the same string options as ContainsValue");
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenTheTimeLiesWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, DateTime> subject = new() { ["a"] = new DateTime(2024, 1, 1, 0, 0, 1), };

				async Task Act()
					=> await That(subject).Contains("a", new DateTime(2024, 1, 1)).Within(1.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheValueIsNullable_ShouldApplyTheTolerance()
			{
				Dictionary<string, double?> subject = new() { ["a"] = 1.05, ["b"] = null, };

				async Task Act()
					=> await That(subject).Contains("a", 1.0).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheValueLiesOutsideTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.2, };

				async Task Act()
					=> await That(subject).Contains("a", 1.0).Within(0.1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1.0 ± 0.1,
					             but it contained key "a" with value 1.2

					             Dictionary:
					             {["a"] = 1.2}
					             """);
			}

			[Fact]
			public async Task WhenTheValueLiesWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, };

				async Task Act()
					=> await That(subject).Contains("a", 1.0).Within(0.1);

				await That(Act).DoesNotThrow()
					.Because("the value of an entry has the same tolerance as the item of a collection");
			}

			[Fact]
			public async Task WhenTheValueOfAPairLiesWithinTheTolerance_ShouldSucceed()
			{
				IReadOnlyDictionary<string, decimal> subject = new Dictionary<string, decimal> { ["a"] = 1.05m, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, decimal>("a", 1.0m)).Within(0.1m);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForADictionary_WithKeyAndValue_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				Dictionary<string, int> result = await That(subject).Contains("a", 1);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForAListOfPairs_ShouldStillBindToTheCollectionOverload()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1),];

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1)).Once();

				await That(Act).DoesNotThrow()
					.Because("a collection of pairs that is no dictionary keeps the quantifier of the collection result");
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}

			[Fact]
			public async Task ForASortedDictionary_WithKeyAndValue_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).Contains("a", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overloads need the same priority to stay unambiguous");
			}

			[Fact]
			public async Task ForASortedDictionaryOfDoubles_WithKeyAndValue_ShouldBindToTheToleranceOverload()
			{
				SortedDictionary<string, double> subject = new() { { "a", 1.05 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, double>,
							IThat<IDictionary<string, double>?>, double, double>)
						That(subject).Contains("a", 1.0).Within(0.1);

				await That(Act).DoesNotThrow()
					.Because("the tolerance overloads of both dictionary interfaces must not become ambiguous");
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Fact]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				Dictionary<string, int> subject = new(comparer) { ["a"] = 1, ["b"] = 2, };
				comparer.IsArmed = true;

				async Task Act()
					=> await That(subject).Contains("a", 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue) { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).Contains("a", 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}
		}
	}
}
