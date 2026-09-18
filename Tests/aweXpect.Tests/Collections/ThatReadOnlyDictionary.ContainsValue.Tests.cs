using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class ContainsValue
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNullValueDoesNotExist_ShouldFail()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value <null>,
					             but it did not contain <null>

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Fact]
			public async Task WhenNullValueExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, null, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(null);

				await That(Act).DoesNotThrow();
			}
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject).ContainsValue("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValueExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(42);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).ContainsValue(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains value 2,
					             but it did not contain 2

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}
		}
	}
}
