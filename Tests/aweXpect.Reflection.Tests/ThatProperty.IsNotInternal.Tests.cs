using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			[Arguments("ProtectedInternalProperty")]
			[Arguments("PrivateProtectedProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
