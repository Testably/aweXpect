using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNullableReferenceType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNullableValueType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableValueProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNullableGenericType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableGenericProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsWriteOnlyNullable_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableWriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNullableInMixedClass_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithMixedNullableMembers)
					.GetProperty(nameof(ClassWithMixedNullableMembers.NullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNonNullableReferenceType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNonNullableValueType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableValueProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNonNullableGenericType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableGenericProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNonNullableInMostlyNullableClass_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithMostlyNullableMembers)
					.GetProperty(nameof(ClassWithMostlyNullableMembers.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsOblivious_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithObliviousMembers)
					.GetProperty(nameof(ClassWithObliviousMembers.ObliviousProperty))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is nullable,
					              but it was non-nullable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is nullable,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotNullable_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNullable_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not nullable,
					              but it was nullable {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
