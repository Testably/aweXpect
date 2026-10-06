using System.Collections.Generic;
using System.Collections.ObjectModel;
using aweXpect.Core;
using aweXpect.Customization;
#if NETFRAMEWORK
using System.Collections.Concurrent;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a dictionary that is there is not equal to a null dictionary");
			}

			[Test]
			public async Task WhenSubjectHasADifferentValue_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the value for key b differs");
			}

			[Test]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> unexpected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

#if NETFRAMEWORK
			[Test]
			public async Task WhenSubjectIsAConcurrentDictionaryOnNetFramework_WithTwoUnexpectedKeysForOneEntry_ShouldSucceed()
			{
				IDictionary<string, int> subject =
					new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["a"] = 1, ["b"] = 1, };
				IDictionary<string, int> unexpected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the key \"b\" matched no unexpected key");
			}

#endif
			[Test]
			public async Task WhenSubjectIsACustomDictionaryWrapper_WithADifferentlyCasedKey_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = new ThatReadOnlyDictionary.ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "A", 1 }, });
				IDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("without the comparer of the wrapped dictionary, the key \"A\" matched no unexpected key");
			}

			[Test]
			public async Task WhenSubjectIsAReadOnlyDictionaryWrapper_WithADifferentlyCasedKey_ShouldFail()
			{
				IDictionary<string, int> subject = new ReadOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "A", 1 }, });
				IDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was

					             Dictionary:
					             {["A"] = 1}
					             """)
					.Because("the comparer of the wrapped dictionary matches the key \"A\" with the unexpected key \"a\"");
			}

			[Test]
			public async Task WhenSubjectIsAReadOnlyDictionaryWrapper_WithTwoUnexpectedKeysForOneEntry_ShouldSucceed()
			{
				IDictionary<string, int> subject = new ReadOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, });
				IDictionary<string, int> unexpected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("both unexpected keys are matched by the key \"a\", and the key \"b\" matched none");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a null dictionary is not equal to a dictionary that is there");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldHandOutANullableSubject()
			{
				Dictionary<int, int>? subject = null;

				Dictionary<int, int>? result = await That(subject).IsNotEqualTo(new Dictionary<int, int>
				{
					{ 1, 1 },
				});

				await That(result).IsNull();
			}

			[Test]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoUnexpectedKeysForOneEntry_ShouldSucceed()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, };
				IDictionary<string, int> unexpected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("both unexpected keys are matched by the key \"a\", so the key \"b\" is left over");
			}

			[Test]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				Dictionary<int, DateTime> unexpected = new() { [1] = value.AddMilliseconds(500), };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).IsNotEqualTo(unexpected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to dictionary unexpected ± 0:01,
					              but it was

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the values fall back to the default tolerance, as the items of a collection do");
			}

			[Test]
			public async Task WhenUnexpectedContainsADuplicateKeyWithADifferentValue_ShouldThrowArgumentException()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> unexpected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("no dictionary can hold both entries, so the expectation would pass for every subject");
			}

			[Test]
			public async Task WhenUnexpectedContainsADuplicateKeyWithTheSameValue_ShouldThrowArgumentException()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> unexpected = [new("a", 1), new("a", 1),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("repeating an entry says nothing that the first one did not already say");
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task WhenTheValuesDifferOnlyInCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };
				Dictionary<int, string> unexpected = new() { [1] = "LET IT BE", };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected ignoring case,
					             but it was

					             Dictionary:
					             {
					               [1] = "Let It Be"
					             }
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Test]
			public async Task WhenAValueLiesOutsideTheTolerance_ShouldSucceed()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.2, };
				Dictionary<string, double> unexpected = new() { ["a"] = 1.0, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTheValuesLieWithinTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, };
				Dictionary<string, double> unexpected = new() { ["a"] = 1.0, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected ± 0.1,
					             but it was

					             Dictionary:
					             {["a"] = 1.05}
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Test]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };
				Dictionary<string, int> unexpected = new() { { "a", 2 }, };

				Dictionary<string, int>? result = await That(subject).IsNotEqualTo(unexpected);

				await That(result).IsSameAs(subject);
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
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}

			[Test]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				Dictionary<string, int> subject = new(comparer) { ["a"] = 1, ["b"] = 2, };
				comparer.IsArmed = true;
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}

			[Test]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue) { ["a"] = 1, ["b"] = 2, };
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}
		}
	}
}
