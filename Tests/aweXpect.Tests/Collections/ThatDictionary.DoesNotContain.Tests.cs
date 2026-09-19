using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class DoesNotContain
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 2));

				await That(Act).DoesNotThrow()
					.Because("the entry for key a holds another value");
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("b", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary has no entry for key b");
			}

			[Fact]
			public async Task WhenPairExists_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

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
				IDictionary<string, int>? subject = null;

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

		public sealed class ComparerTests
		{
			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("A", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["A"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}
	}
}
