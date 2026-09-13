using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContainKeys
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllKeysDoNotExist_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 43);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAtLeastOneKeyExists_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys [42, 2],
					             but it did contain [
					               2
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContainKeys("foo", "bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainKeys(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenOneKeyIsMissingAndOneExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainKeys(42, 2));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
