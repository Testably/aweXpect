using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalProperty", "internal")]
			[Arguments("PublicProperty", "public")]
			[Arguments("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName,
				string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
