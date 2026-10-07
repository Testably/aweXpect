using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedContainsADuplicateKeyWithADifferentValue_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("a dictionary holds one value per key, so no subject could ever satisfy both entries");
			}

			[Test]
			public async Task WhenExpectedContainsADuplicateKeyWithTheSameValue_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 1),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("repeating an entry says nothing that the first one did not already say");
			}

			[Test]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> expected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a dictionary is a keyed lookup without a contractual enumeration order");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoExpectedKeysForOneEntry_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"a", 1
						},
						{
							"b", 1
						},
					};
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked a distinct key for "A" and contained additional key "b"

					             Dictionary:
					             {["a"] = 1, ["b"] = 1}
					             """)
					.Because("both expected keys are matched by the key \"a\" of the subject");
			}

			[Test]
			public async Task WhenTheValueForAKeyDiffers_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "a" with value 1 instead of 2

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class ReadOnlyOnlyTests
		{
			[Test]
			public async Task WhenSubjectHasAnAdditionalKey_ShouldFail()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>
					{
						{
							"a", 1
						},
						{
							"b", 2
						},
					});
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained additional key "b"

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

			[Test]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeysUpThroughIt()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"a", 1
						},
					});
				IReadOnlyDictionary<string, int> expected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IReadOnlyDictionary<string, int>?,
						IThat<IReadOnlyDictionary<string, int>?>, int, int>)That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "a" that matched no expected key

					             Dictionary:
					             {["a"] = 1}
					             """)
					.Because("the expected key \"A\" is found through its own TryGetValue, but without its comparer the key \"a\" cannot be told apart from an additional key");
			}

			[Test]
			public async Task WhenTheComparerCannotBeRead_WithAnAdditionalKey_ShouldNotNameIt()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"a", 1
						},
						{
							"b", 1
						},
					});
				IReadOnlyDictionary<string, int> expected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained 2 keys and matched 1 expected key

					             Dictionary:
					             {["a"] = 1, ["b"] = 1}
					             """)
					.Because("without the comparer of the subject, naming the additional keys would overshoot");
			}

			[Test]
			public async Task WhenTheComparerCannotBeRead_WithTwoExpectedKeysForOneEntry_ShouldFail()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
					{
						{
							"a", 1
						},
						{
							"b", 1
						},
					});
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "b" that matched no expected key

					             Dictionary:
					             {["a"] = 1, ["b"] = 1}
					             """)
					.Because("without the comparer of the subject, the two expected keys must not hide the key \"b\"");
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task WhenTheValuesDifferOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<int, string> subject = new(new Dictionary<int, string>
				{
					{
						1, "Let It Be"
					},
				});
				IReadOnlyDictionary<int, string> expected = ToDictionary([1,], ["LET IT BE",]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithinTests
		{
			[Test]
			public async Task WhenTheValuesLieWithinTheTolerance_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, double> subject = new(new Dictionary<string, double>
				{
					{
						"a", 1.05
					},
				});
				IReadOnlyDictionary<string, double> expected = ToDictionary(["a",], [1.0,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration)
					{
						["a"] = 1,
						["b"] = 2,
					});
				Dictionary<string, int> expected = new()
				{
					["a"] = 1,
					["b"] = 2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(comparer)
					{
						["a"] = 1,
						["b"] = 2,
					});
				comparer.IsArmed = true;
				Dictionary<string, int> expected = new()
				{
					["a"] = 1,
					["b"] = 2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue)
					{
						["a"] = 1,
						["b"] = 2,
					});
				Dictionary<string, int> expected = new()
				{
					["a"] = 1,
					["b"] = 2,
				};

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
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
