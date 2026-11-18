using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed class AreNullEnumerable
		{
			public sealed class Tests
			{
				[Test]
				public async Task AreNotNull_WhenNoItemIsNull_ShouldSucceed()
				{
					IEnumerable subject = new object[] { 1, "foo", };

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNotNull_WhenOneItemIsNull_ShouldFail()
				{
					IEnumerable subject = new object?[] { 1, null, "foo", };

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [<null>]

						             Collection:
						             [1, <null>, "foo"]
						             """);
				}

				[Test]
				public async Task AreNull_WhenAllItemsAreNull_ShouldSucceed()
				{
					IEnumerable subject = new object?[] { null, null, };

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNull_WhenOneItemIsNotNull_ShouldFail()
				{
					IEnumerable subject = new object?[] { null, 1, };

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [<null>, 1]
						             """);
				}
			}
		}
	}
}
