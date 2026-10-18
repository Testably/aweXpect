using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty", "protected")]
			[Arguments("PublicProperty", "public")]
			[Arguments("InternalProperty", "internal")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName,
				string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("InternalProperty")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PrivateProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
