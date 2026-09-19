using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenBothAreNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).DoesNotThrow()
					.Because("a dictionary that is there is not equal to a null dictionary");
			}

			[Fact]
			public async Task WhenSubjectHasADifferentValue_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the value for key b differs");
			}

			[Fact]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> unexpected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a null dictionary is not equal to a dictionary that is there");
			}
		}
	}
}
