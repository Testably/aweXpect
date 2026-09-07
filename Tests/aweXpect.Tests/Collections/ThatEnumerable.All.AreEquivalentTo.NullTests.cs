using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEquivalentTo
		{
			public sealed class NullTests
			{
				[Fact]
				public async Task WhenAllItemsAreNull_ShouldSucceed()
				{
					IEnumerable<int?> subject = new int?[]
					{
						null, null,
					};

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreNotNull_ShouldFail()
				{
					IEnumerable<int?> subject = new int?[]
					{
						null, 1,
					};

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to null for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [<null>, 1]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int?>? subject = null!;

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to null for all items,
						             but it was <null>

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task ForNonGenericEnumerable_WhenAllItemsAreNull_ShouldSucceed()
				{
					IEnumerable subject = new object?[]
					{
						null, null,
					};

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForNonGenericEnumerable_WhenSomeItemsAreNotNull_ShouldFail()
				{
					IEnumerable subject = new object?[]
					{
						null, "foo",
					};

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to null for all items,
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

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task ForStructEnumerable_WhenAllItemsAreNull_ShouldSucceed()
				{
					ImmutableArray<int?> subject = [null, null,];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForStructEnumerable_WhenSomeItemsAreNotNull_ShouldFail()
				{
					ImmutableArray<int?> subject = [null, 1,];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to null for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [<null>, 1]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
#endif
			}
		}
	}
}
