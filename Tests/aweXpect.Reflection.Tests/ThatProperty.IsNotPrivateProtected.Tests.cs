using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
