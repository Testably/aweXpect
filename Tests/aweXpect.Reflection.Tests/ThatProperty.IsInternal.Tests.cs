using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedProperty", "protected")]
			[Arguments("PublicProperty", "public")]
			[Arguments("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName,
				string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}

			[Test]
			[Arguments("ProtectedProperty")]
			[Arguments("PublicProperty")]
			[Arguments("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
