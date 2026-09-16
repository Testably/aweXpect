using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 2,]);

					async Task Act()
						=> await That(subject).None().AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 1, 3,]);

					async Task Act()
						=> await That(subject).None().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for no items,
						             but 2 of 4 were

						             Matching items:
						             [2, 3]

						             Collection:
						             [1, 2, 1, 3]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).None().AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for no items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
