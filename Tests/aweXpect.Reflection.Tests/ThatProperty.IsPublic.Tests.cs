using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty", "protected")]
			[InlineData("InternalProperty", "internal")]
			[InlineData("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("InternalProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("Expected that subject*")
					.AsWildcard();
			}
		}
	}
}
