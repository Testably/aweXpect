using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNonNullableReferenceType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNonNullableValueType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableValueProperty))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsOblivious_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithObliviousMembers)
					.GetProperty(nameof(ClassWithObliviousMembers.ObliviousProperty))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNullableReferenceType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableProperty))!;

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
			public async Task WhenPropertyIsNullableValueType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableValueProperty))!;

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
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsNotNullable_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableProperty))!;

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
			public async Task WhenPropertyIsNullable_ShouldSucceed()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
