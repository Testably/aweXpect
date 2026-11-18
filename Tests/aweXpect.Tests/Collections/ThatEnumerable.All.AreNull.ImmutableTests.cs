#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed class AreNullImmutable
		{
			public sealed class Tests
			{
				[Test]
				public async Task AreNotNull_WhenNoItemIsNull_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["foo", "bar",];

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNotNull_WhenOneItemIsNull_ShouldFail()
				{
					ImmutableArray<string?> subject = ["foo", null, "bar",];

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [
						               <null>
						             ]

						             Collection:
						             [
						               "foo",
						               <null>,
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task AreNull_WhenAllItemsAreNull_ShouldSucceed()
				{
					ImmutableArray<string?> subject = [null, null,];

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNull_WhenOneItemIsNotNull_ShouldFail()
				{
					ImmutableArray<string?> subject = [null, "foo",];

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               "foo"
						             ]

						             Collection:
						             [
						               <null>,
						               "foo"
						             ]
						             """);
				}
			}
		}
	}
}
#endif
