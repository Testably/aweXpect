using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class OnlyHaveNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task WhenTypesContainTypeWithNonNullableMembers_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithNullableMembers),
					typeof(ClassWithSingleNonNullableProperty),
				];
				PropertyInfo property = typeof(ClassWithSingleNonNullableProperty)
					.GetProperty(nameof(ClassWithSingleNonNullableProperty.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only have nullable members,
					              but it contained types with non-nullable members [
					                ClassWithSingleNonNullableProperty with non-nullable members [{Formatter.Format(property)}]
					              ]
					              """);
			}

			[Fact]
			public async Task WhenMultipleTypesDoNotComply_ShouldListAllWithComma()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ClassWithSingleNonNullableProperty),
					null,
				];
				PropertyInfo property = typeof(ClassWithSingleNonNullableProperty)
					.GetProperty(nameof(ClassWithSingleNonNullableProperty.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only have nullable members,
					              but it contained types with non-nullable members [
					                ClassWithSingleNonNullableProperty with non-nullable members [{Formatter.Format(property)}],
					                <null>
					              ]
					              """);
			}

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldListNullWithoutViolations()
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only have nullable members,
					             but it contained types with non-nullable members [
					               <null>
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all only have nullable members,
					             but it only contained types with only nullable members [
					               ClassWithSingleNullableProperty
					             ]
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenAllTypesOnlyHaveNullableMembers_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNullableMembers),
						typeof(ClassWithSingleNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypesContainTypeWithNonNullableMembers_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithNullableMembers),
						typeof(ClassWithSingleNonNullableProperty),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).OnlyHaveNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only have nullable members,
					             but it contained types with non-nullable members [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
