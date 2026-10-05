using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

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
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary holds the key with that value");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it was <null>
					             """);
			}
		}

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains("a", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

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
		}

		public sealed class ReadOnlyOnlyTests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });

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
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, });

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 1));

				await That(Act).DoesNotThrow()
					.Because("a type that implements no IDictionary is looked up through its own TryGetValue");
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenTheValueDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<int, string> subject = new(new Dictionary<int, string> { { 1, "Let It Be" }, });

				async Task Act()
					=> await That(subject).Contains(1, "let it be").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenTheValueLiesWithinTheTolerance_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, double> subject = new(new Dictionary<string, double> { { "a", 1.05 }, });

				async Task Act()
					=> await That(subject).Contains("a", 1.0).Within(0.1);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForAReadOnlyDictionary_WithKeyAndValue_ShouldKeepTheSubjectType()
			{
				ReadOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });

				ReadOnlyDictionary<string, int> result = await That(subject).Contains("a", 1);

				await That(result).IsSameAs(subject);
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Fact]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(comparer) { ["a"] = 1, ["b"] = 2, });
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
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue) { ["a"] = 1, ["b"] = 2, });

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
