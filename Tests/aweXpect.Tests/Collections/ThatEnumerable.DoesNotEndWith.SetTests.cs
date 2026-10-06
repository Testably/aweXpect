using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotEndWith
	{
		public sealed class SetTests
		{
			[Test]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotEndWith("B").Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSetDoesNotEndWithTheItemsAccordingToItsComparer_ShouldSucceed()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotEndWith("A");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSetEndsWithTheItemsAccordingToItsComparer_ShouldFail()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotEndWith("B");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["B"] using the subject's StringComparer.OrdinalIgnoreCase,
					             but it did end with [
					               "b"
					             ]
					             """);
			}
		}
	}
}