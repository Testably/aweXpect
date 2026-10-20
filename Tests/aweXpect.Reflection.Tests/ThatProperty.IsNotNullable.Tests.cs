using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotNullable
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not nullable,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNullableReferenceType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableProperty))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not nullable,
					              but it was nullable public string ClassWithNullableMembers.NullableProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNullableValueType_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNullableMembers)
					.GetProperty(nameof(ClassWithNullableMembers.NullableValueProperty))!;

				async Task Act()
				{
					await That(subject).IsNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not nullable,
					              but it was nullable public int? ClassWithNullableMembers.NullableValueProperty { get; set; }
					              """);
			}

			[Test]
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsNotNullable_ShouldFail()
			{
				PropertyInfo subject = typeof(ClassWithNonNullableMembers)
					.GetProperty(nameof(ClassWithNonNullableMembers.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotNullable());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable public string ClassWithNonNullableMembers.NonNullableProperty { get; set; }
					              """);
			}

			[Test]
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
