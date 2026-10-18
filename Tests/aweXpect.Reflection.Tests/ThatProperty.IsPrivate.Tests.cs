using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedProperty", "protected")]
			[Arguments("PublicProperty", "public")]
			[Arguments("InternalProperty", "internal")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName,
				string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivate_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PrivateProperty");

				async Task Act()
				{
					await That(subject).IsPrivate();
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
			public async Task WhenPropertyInfoIsNotPrivate_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInfoIsPrivate_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PrivateProperty");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
