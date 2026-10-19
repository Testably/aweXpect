using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsRequired
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldIsNotRequired_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.OptionalField))!;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is required,
					              but it was non-required string ClassWithRequiredMembers.OptionalField
					              """);
			}

			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is required,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldIsRequired_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.RequiredField))!;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldIsNotRequired_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.OptionalField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsRequired_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.RequiredField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not required,
					              but it was required string ClassWithRequiredMembers.RequiredField
					              """);
			}
		}
	}
}
