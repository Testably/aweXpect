using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContainKey
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExists_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKey(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain key 2,
					             but it did

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKey(42);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContainKey("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain key "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["foo",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContainKey(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}
	}
}
