using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Exactly
	{
		public sealed class AreExactly
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExactlyTheExpectedNumberOfItemsIsOfType_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).Exactly(2).AreExactly<int>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsOnlyInheritFromType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).Exactly(2).AreExactly<ValueType>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type ValueType for exactly 2 items,
						             but none of 3 were

						             Collection:
						             [
						               1,
						               "a",
						               2
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<object>? subject = null;

					async Task Act()
						=> await That(subject).Exactly(2).AreExactly<int>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type int for exactly 2 items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreOfType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).Exactly(1).AreExactly<int>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type int for exactly one item,
						             but at least 2 of at least 3 were

						             Collection:
						             [
						               1,
						               "a",
						               2
						             ]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenExactlyTheExpectedNumberOfItemsIsOfType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Exactly(2).AreExactly<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is exactly of type int for not exactly 2 items,
						             but 2 of 3 were

						             Collection:
						             [
						               1,
						               "a",
						               2
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreOfType_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Exactly(1).AreExactly<int>());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
