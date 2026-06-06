using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNonNullableReferenceType_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableField))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNonNullableValueType_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableValueField))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsOblivious_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithObliviousMembers)
					.GetField(nameof(ClassWithObliviousMembers.ObliviousField))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNullableReferenceType_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableField))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not nullable,
					              but it was nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNullableValueType_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableValueField))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not nullable,
					              but it was nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not nullable,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsNotNullable_ShouldFail()
			{
				FieldInfo subject = typeof(ClassWithNonNullableMembers)
					.GetField(nameof(ClassWithNonNullableMembers.NonNullableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNullable_ShouldSucceed()
			{
				FieldInfo subject = typeof(ClassWithNullableMembers)
					.GetField(nameof(ClassWithNullableMembers.NullableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
