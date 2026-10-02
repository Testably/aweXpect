using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class Are
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenNoItemIsOfType_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>("a", "b");

					async Task Act()
						=> await That(subject).None().Are<int>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreOfType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).None().Are<int>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type int for no items,
						             but at least 1 of at least 1 were

						             Matching items:
						             [
						               1,
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               1,
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<object>? subject = null;

					async Task Act()
						=> await That(subject).None().Are<int>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type int for no items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenNoItemIsOfType_ShouldFail()
				{
					IEnumerable<object> subject = ToEnumerable<object>("a", "b");

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.None().Are<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type int for at least one item,
						             but none of 2 were

						             Collection:
						             [
						               "a",
						               "b"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenSomeItemsAreOfType_ShouldSucceed()
				{
					IEnumerable<object> subject = ToEnumerable<object>(1, "a", 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.None().Are<int>());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
