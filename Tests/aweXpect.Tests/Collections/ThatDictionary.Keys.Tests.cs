using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Keys
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationOnKeysFails_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Keys.Contains(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 0 at least once,
					             but it did not contain it

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Keys.Contains(2).And.IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNegatedExpectationOnKeysFails_ShouldFail()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain an item equal to 2,
					             but it contained 2 at least once
					             """);
			}

			[Fact]
			public async Task WhenNegatedExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				IDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(4));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain an item equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Keys.Contains(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 0 at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class DictionaryTests
		{
			[Fact]
			public async Task WhenExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				Dictionary<int, string> subject = new()
				{
					[1] = "foo",
					[2] = "bar",
				};

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Dictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain an item equal to 2 at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class KeyComparerTests
		{
#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForAnImmutableDictionary_ShouldUseTheKeyComparer()
			{
				IDictionary<string, int> subject = ImmutableDictionary.Create<string, int>(StringComparer.OrdinalIgnoreCase)
					.Add("a", 1);

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task ForASortedDictionary_ShouldOrderByTheKeyComparer()
			{
				SortedDictionary<string, int> subject = new(new ReverseComparer())
				{
					{ "a", 1 },
					{ "b", 2 },
				};

				async Task Act()
					=> await That(subject).Keys.IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IsEqualTo_ShouldUseTheKeyComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 2 }, };

				async Task Act()
					=> await That(subject).Keys.IsEqualTo(["A", "B",]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseTheKeyComparer()
			{
				IDictionary<string, int> subject = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
				{
					{ "a", 1 },
				};

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheKeyComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Keys.Contains("a").Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "a" using AllDifferentComparer at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenKeysContainTheKeyAccordingToTheKeyComparer_NegatedShouldFail()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Keys.DoesNotContain("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that do not contain "A" using the subject's StringComparer.OrdinalIgnoreCase,
					             but it contained "A" once

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenKeysDoNotContainTheKeyAccordingToTheKeyComparer_ShouldNameTheComparer()
			{
				Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Keys.Contains("b");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "b" using the subject's StringComparer.OrdinalIgnoreCase at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WithTheDefaultKeyComparer_ShouldUseTheDefaultEquality()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Keys.Contains("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys that contain "A" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForASortedDictionary_ShouldNotBeAmbiguous()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Keys.Contains("a");

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
