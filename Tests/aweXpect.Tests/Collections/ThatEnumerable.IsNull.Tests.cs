using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class IsNull
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(null)]
			[Arguments(new int[0])]
			public async Task IsNull_ShouldBeChainableWithIsEmpty(int[]? subject)
			{
				async Task Act()
					=> await That(subject)
						.IsNull().Or.IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IsNull_WhenNotNull_ShouldFail()
			{
				List<int> subject = [];

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was []
					             """);
			}

			[Test]
			public async Task IsNull_WhenNull_ShouldSucceed()
			{
				List<int>? subject = null;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
