using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class OnlyHaveNonNullableMembers
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNonNullableMembers_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNonNullableMembers),
					typeof(ClassWithSingleNonNullableProperty),
					typeof(ClassWithoutMembers),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNonNullableMembers),
					null,
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all only have non-nullable members,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithNonNullableMembers,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenMoreTypesHaveNullableMembersThanTheMaximumNumberOfCollectionItems_ShouldLimitTheList()
			{
				using IDisposable _ = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(1);
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithSingleNullableProperty),
					typeof(ClassWithSingleNullableEvent),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all only have non-nullable members,
					             but none of 2 did

					             Not matching items:
					             [
					               ClassWithSingleNullableProperty with nullable members [public string ClassWithSingleNullableProperty.NullableProperty { get; set; }],
					               (… and 1 more)
					             ]

					             Collection:
					             [
					               ClassWithSingleNullableProperty with nullable members [public string ClassWithSingleNullableProperty.NullableProperty { get; set; }],
					               (… and 1 more)
					             ]
					             """)
					.Because("the not matching types are limited like any other collection in a failure message");
			}

			[Test]
			public async Task WhenTypesContainTypeWithNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNonNullableMembers),
					typeof(ClassWithSingleNullableProperty),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all only have non-nullable members,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNonNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithSingleNonNullableProperty),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.OnlyHaveNonNullableMembers());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all only have non-nullable members,
					             but all 1 did

					             Collection:
					             [
					               ClassWithSingleNonNullableProperty
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesContainTypeWithNullableMembers_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNonNullableMembers),
					typeof(ClassWithSingleNullableProperty),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.OnlyHaveNonNullableMembers());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNonNullableMembers_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNonNullableMembers), typeof(ClassWithSingleNonNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainTypeWithNullableMembers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNonNullableMembers), typeof(ClassWithSingleNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all only have non-nullable members,
					             but only * of * did

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
