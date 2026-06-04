using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotRequired_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

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
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsRequired_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.RequiredProperty))!;

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
			public async Task WhenPropertyIsNotRequired_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsRequired_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.RequiredProperty))!;

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
