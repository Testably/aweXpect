using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
