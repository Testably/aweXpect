#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEquivalentTo
		{
			public sealed class NullTests
			{
				[Test]
				public async Task WhenAllItemsAreNull_ShouldSucceed()
				{
					IAsyncEnumerable<int?> subject = Factory.GetConstantValueAsyncEnumerable<int?>(null, 3);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsAreNotNull_ShouldFail()
				{
					IAsyncEnumerable<int?> subject = Factory.GetConstantValueAsyncEnumerable<int?>(1, 3);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to <null> for all items,
						             but none of at least 1 were

						             Not matching items:
						             [1, (… and maybe more)]

						             Collection:
						             [1, (… and maybe more)]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int?>? subject = null!;

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to <null> for all items,
						             but it was <null>

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
			}
		}
	}
}
#endif
