using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class AreNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenNoItemIsNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = ["foo", "bar",];

					async Task Act()
						=> await That(subject).None().AreNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneItemIsNull_ShouldFail()
				{
					IEnumerable<string?> subject = ["foo", null, "bar",];

					async Task Act()
						=> await That(subject).None().AreNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null for no items,
						             but 1 of 3 were

						             Matching items:
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
			}
		}

		public sealed class AreNotNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenAllItemsAreNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = [null, null,];

					async Task Act()
						=> await That(subject).None().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneItemIsNotNull_ShouldFail()
				{
					IEnumerable<string?> subject = [null, "foo", null,];

					async Task Act()
						=> await That(subject).None().AreNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null for no items,
						             but 1 of 3 were

						             Matching items:
						             [
						               "foo"
						             ]

						             Collection:
						             [
						               <null>,
						               "foo",
						               <null>
						             ]
						             """);
				}
			}
		}
	}
}
