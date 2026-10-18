using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldFail(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("ProtectedInternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
