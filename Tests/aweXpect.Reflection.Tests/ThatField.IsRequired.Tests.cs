using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNotRequired_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.OptionalField))!;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is required,
					              but it was non-required {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is required,
					             but it was <null>
					             """);
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenFieldIsRequired_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithRequiredMembers).GetField(nameof(ClassWithRequiredMembers.RequiredField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not required,
					              but it was required {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
