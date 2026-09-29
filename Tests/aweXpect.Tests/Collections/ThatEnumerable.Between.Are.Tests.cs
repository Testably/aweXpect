using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Between
	{
		public sealed class Are
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenNumberOfItemsOfTypeIsInRange_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).Between(1).And(2).Are<string>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<object>? subject = null;

					async Task Act()
						=> await That(subject).Between(1).And(2).Are<string>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type string for between 1 and 2 items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTooFewItemsAreOfType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, 2, 3);

					async Task Act()
						=> await That(subject).Between(1).And(2).Are<string>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type string for between 1 and 2 items,
						             but none of 3 were

						             Collection:
						             [
						               1,
						               2,
						               3
						             ]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenNumberOfItemsOfTypeIsInRange_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Between(1).And(2).Are<string>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type string for not between 1 and 2 items,
						             but 1 of 3 were

						             Collection:
						             [
						               1,
						               "a",
						               2
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTooFewItemsAreOfType_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Between(1).And(2).Are<string>());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
