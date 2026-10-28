using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class OnlyHaveNullableMembers
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Directly_ShouldOnlyConsiderTheDeclaredMembers()
			{
				IEnumerable<Type?> subject =
					[typeof(DerivedClassWithNullableMembers), typeof(ClassWithSingleNonNullableProperty),];

				async Task Act()
				{
					await That(subject).AtLeast(1).OnlyHaveNullableMembers().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_WhenTooFewItemsOnlyHaveNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithNullableMembers), typeof(ClassWithSingleNonNullableProperty),];

				async Task Act()
				{
					await That(subject).AtLeast(2).OnlyHaveNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has nullable members for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }]
					             ]

					             Collection:
					             [
					               ClassWithNullableMembers,
					               ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }]
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithNullableMembers), null,];

				async Task Act()
				{
					await That(subject).AtMost(1).OnlyHaveNullableMembers();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtMost(1).OnlyHaveNullableMembers());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has nullable members for at most one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithNullableMembers,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has nullable members for more than one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithNullableMembers,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task None_WhenAnItemOnlyHasNullableMembers_ShouldFail()
			{
				ImmutableArray<Type?> subject =
					[typeof(ClassWithSingleNonNullableProperty), typeof(ClassWithSingleNullableProperty),];

				async Task Act()
				{
					await That(subject).None().OnlyHaveNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has nullable members for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ClassWithSingleNullableProperty
					             ]

					             Collection:
					             [
					               ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }],
					               ClassWithSingleNullableProperty
					             ]
					             """);
			}
		}

		public sealed class QuantifiedAsyncEnumerableTests
		{
			[Test]
			public async Task Exactly_WhenTheNumberOfItemsDiffers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithNullableMembers), typeof(ClassWithSingleNonNullableProperty),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).Exactly(2).OnlyHaveNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has nullable members for exactly 2 items,
					             but only 1 of 2 did

					             Collection:
					             [
					               ClassWithNullableMembers,
					               ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }]
					             ]
					             """);
			}
		}
#endif
	}

	public sealed partial class OnlyHaveNonNullableMembers
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsOnlyHaveNonNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithNonNullableMembers), typeof(ClassWithSingleNullableProperty),];

				async Task Act()
				{
					await That(subject).AtLeast(2).OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has non-nullable members for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ClassWithSingleNullableProperty with nullable members [public string ClassWithSingleNullableProperty.NullableProperty { get; set; }]
					             ]

					             Collection:
					             [
					               ClassWithNonNullableMembers,
					               ClassWithSingleNullableProperty with nullable members [public string ClassWithSingleNullableProperty.NullableProperty { get; set; }]
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_IncludingObjectMembers_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithNonNullableMembers), typeof(ClassWithSingleNullableProperty),];

				async Task Act()
				{
					await That(subject).Exactly(1).OnlyHaveNonNullableMembers().IncludingObjectMembers();
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsOnlyHaveNonNullableMembers_ShouldSucceed()
			{
				ImmutableArray<Type?> subject =
					[typeof(ClassWithNonNullableMembers), typeof(ClassWithSingleNullableProperty),];

				async Task Act()
				{
					await That(subject).AtLeast(1).OnlyHaveNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class QuantifiedAsyncEnumerableTests
		{
			[Test]
			public async Task None_WhenAnItemOnlyHasNonNullableMembers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithNonNullableMembers),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).None().OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             only has non-nullable members for no items,
					             but at least 1 of at least 1 did

					             Matching items:
					             [
					               ClassWithNonNullableMembers,
					               (… and maybe more)
					             ]

					             Collection:
					             [
					               ClassWithNonNullableMembers,
					               (… and maybe more)
					             ]
					             """);
			}
		}
#endif
	}
}
