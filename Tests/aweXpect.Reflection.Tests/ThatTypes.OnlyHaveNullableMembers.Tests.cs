using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class OnlyHaveNullableMembers
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNullableMembers_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNullableMembers),
					typeof(ClassWithSingleNullableProperty),
					typeof(ClassWithoutMembers),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBaseTypeHasNonNullableMembers_Directly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithNullableMembers),
				};

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the members declared on the types count");
			}

			[Test]
			public async Task WhenBaseTypeHasNonNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithNullableMembers),
				};

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all only have nullable members,
					             but none of * did

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard()
					.Because("inherited members are considered by default");
			}

			[Test]
			public async Task WhenCollectionContainsNull_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNullableMembers),
					null,
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all only have nullable members,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ClassWithNullableMembers,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenMultipleTypesDoNotComply_ShouldListAllWithComma()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithSingleNonNullableProperty),
					typeof(ClassWithSingleNonNullableEvent),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all only have nullable members,
					              but none of 2 did

					              Not matching items:
					              [
					                ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }],
					                ClassWithSingleNonNullableEvent with non-nullable members [event EventHandler ClassWithSingleNonNullableEvent.NonNullableEvent]
					              ]

					              Collection:
					              [
					                ClassWithSingleNonNullableProperty with non-nullable members [public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }],
					                ClassWithSingleNonNullableEvent with non-nullable members [event EventHandler ClassWithSingleNonNullableEvent.NonNullableEvent]
					              ]
					              """);
			}

			[Test]
			public async Task WhenTypesContainTypeWithNonNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNullableMembers),
					typeof(ClassWithSingleNonNullableProperty),
				];

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              all only have nullable members,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithSingleNullableProperty),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.OnlyHaveNullableMembers());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all only have nullable members,
					             but all 1 did

					             Collection:
					             [
					               ClassWithSingleNullableProperty
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesContainTypeWithNonNullableMembers_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNullableMembers),
					typeof(ClassWithSingleNonNullableProperty),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.OnlyHaveNullableMembers());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllTypesOnlyHaveNullableMembers_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNullableMembers), typeof(ClassWithSingleNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainTypeWithNonNullableMembers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNullableMembers), typeof(ClassWithSingleNonNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all only have nullable members,
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
