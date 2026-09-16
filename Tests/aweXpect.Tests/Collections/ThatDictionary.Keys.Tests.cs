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
					             has keys which contain 0 at least once,
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
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).Keys.Contains(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys which contain 0 at least once,
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
					             has keys which contain 2 at least once,
					             but it was <null>
					             """);
			}
		}
	}
}
