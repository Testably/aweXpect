using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty", "protected")]
			[Arguments("InternalProperty", "internal")]
			[Arguments("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
