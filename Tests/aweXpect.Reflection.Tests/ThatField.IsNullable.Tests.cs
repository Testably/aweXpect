using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNonNullableGenericType_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableGenericField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable List<string> ClassWithNonNullableMembers.NonNullableGenericField
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNonNullableInMostlyNullableClass_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithMostlyNullableMembers)
					.GetField(nameof(ClassWithMostlyNullableMembers.NonNullableField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable string ClassWithMostlyNullableMembers.NonNullableField
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNonNullableReferenceType_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable string ClassWithNonNullableMembers.NonNullableField
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNonNullableValueType_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableValueField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable int ClassWithNonNullableMembers.NonNullableValueField
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is nullable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldIsNullableGenericType_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableGenericField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNullableInMixedClass_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithMixedNullableMembers)
					.GetField(nameof(ClassWithMixedNullableMembers.NullableField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNullableReferenceType_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNullableValueType_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableValueField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsOblivious_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithObliviousMembers)
					.GetField(nameof(ClassWithObliviousMembers.ObliviousField))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable string ClassWithObliviousMembers.ObliviousField
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsNotNullable_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNullable_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not nullable,
					              but it was nullable string ClassWithNullableMembers.NullableField
					              """);
			}
		}
	}
}
