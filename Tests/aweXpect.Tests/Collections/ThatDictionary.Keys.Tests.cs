using System.Collections.Generic;

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
