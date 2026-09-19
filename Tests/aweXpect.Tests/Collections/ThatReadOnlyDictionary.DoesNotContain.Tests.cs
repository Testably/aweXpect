using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContain
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("b", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary has no entry for key b");
			}

			[Fact]
			public async Task WhenPairExists_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it was <null>
					             """);
			}
		}
	}
}
