using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
