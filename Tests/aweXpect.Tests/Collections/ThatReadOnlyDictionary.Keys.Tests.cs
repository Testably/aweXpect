using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class Keys
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationOnKeysFails_ShouldFail()
			{
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

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
				IReadOnlyDictionary<int, string> subject = ToDictionary([1, 2, 3,], ["foo", "bar", "baz",]);

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Keys.Contains(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has keys which contain 0 at least once,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<int, string>? subject = null;

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

		public sealed class ReadOnlyDictionaryTests
		{
			[Fact]
			public async Task WhenExpectationOnKeysIsSatisfied_ShouldSucceed()
			{
				ReadOnlyDictionary<int, string> subject = new(new Dictionary<int, string>
				{
					[1] = "foo",
					[2] = "bar",
				});

				async Task Act()
					=> await That(subject).Keys.Contains(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

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
