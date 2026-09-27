using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotStartWith
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotStartWith("A").Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetDoesNotStartWithTheItemsAccordingToItsComparer_ShouldSucceed()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotStartWith("B");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetStartsWithTheItemsAccordingToItsComparer_ShouldFail()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotStartWith("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["A"] using the subject's StringComparer.OrdinalIgnoreCase,
					             but it did start with [
					               "a"
					             ]
					             """);
			}
		}
	}
}