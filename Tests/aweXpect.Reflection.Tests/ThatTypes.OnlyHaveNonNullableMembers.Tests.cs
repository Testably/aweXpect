using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class OnlyHaveNonNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task WhenTypesContainTypeWithNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNonNullableMembers),
					typeof(ClassWithSingleNullableProperty),
				];
				PropertyInfo property = typeof(ClassWithSingleNullableProperty)
					.GetProperty(nameof(ClassWithSingleNullableProperty.NullableProperty))!;

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only have non-nullable members,
					              but it contained types with nullable members [
					                ClassWithSingleNullableProperty with nullable members [{Formatter.Format(property)}]
					              ]
					              """);
			}

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldListNullWithoutViolations()
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only have non-nullable members,
					             but it contained types with nullable members [
					               <null>
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all only have non-nullable members,
					             but it only contained types with only non-nullable members [
					               ClassWithSingleNonNullableProperty
					             ]
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenAllTypesOnlyHaveNonNullableMembers_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNonNullableMembers),
						typeof(ClassWithSingleNonNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesContainTypeWithNullableMembers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNonNullableMembers),
						typeof(ClassWithSingleNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only have non-nullable members,
					             but it contained types with nullable members [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
