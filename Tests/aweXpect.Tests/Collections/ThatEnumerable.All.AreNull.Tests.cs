using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed class AreNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenAllItemsAreNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = [null, null,];

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneItemIsNotNull_ShouldFail()
				{
					IEnumerable<string?> subject = [null, "foo", null,];

					async Task Act()
						=> await That(subject).All().AreNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null for all items,
						             but only 2 of 3 were

						             Not matching items:
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

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenAllItemsAreNull_ShouldFail()
				{
					IEnumerable<string?> subject = [null, null,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNull());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null not for all items,
						             but all 2 were

						             Collection:
						             [
						               <null>,
						               <null>
						             ]
						             """);
				}

				[Test]
				public async Task WhenOneItemIsNotNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = [null, "foo",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNull());

					await That(Act).DoesNotThrow();
				}
			}
		}

		public sealed class AreNotNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenNoItemIsNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = ["foo", "bar",];

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOneItemIsNull_ShouldFail()
				{
					IEnumerable<string?> subject = ["foo", null, "bar",];

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
				public async Task WhenSubjectIsEmpty_ShouldSucceed()
				{
					IEnumerable<string?> subject = [];

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string?>? subject = null;

					async Task Act()
						=> await That(subject).All().AreNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenNoItemIsNull_ShouldFail()
				{
					IEnumerable<string?> subject = ["foo", "bar",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNotNull());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null not for all items,
						             but all 2 were

						             Collection:
						             [
						               "foo",
						               "bar"
						             ]
						             """);
				}

				[Test]
				public async Task WhenOneItemIsNull_ShouldSucceed()
				{
					IEnumerable<string?> subject = ["foo", null,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().AreNotNull());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
