#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class AreNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task AreNotNull_WhenNoItemIsNull_ShouldSucceed()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("foo", "bar");

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNotNull_WhenOneItemIsNull_ShouldFail()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>("foo", null, "bar");

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null for all items,
						             but only 1 of at least 2 were

						             Not matching items:
						             [
						               <null>,
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "foo",
						               <null>,
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task AreNull_WhenAllItemsAreNull_ShouldSucceed()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>([null, null,]);

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task AreNull_WhenOneItemIsNotNull_ShouldFail()
				{
					IAsyncEnumerable<string?> subject = ToAsyncEnumerable<string?>(null, "foo");

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null for all items,
						             but only 1 of at least 2 were

						             Not matching items:
						             [
						               "foo",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               <null>,
						               "foo",
						               (… and maybe more)
						             ]
						             """);
				}
			}
		}
	}
}
#endif
