using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty", "protected")]
			[InlineData("PublicProperty", "public")]
			[InlineData("InternalProperty", "internal")]
			public async Task WhenPropertyInfoIsNotPrivateProtected_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivateProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PrivateProtectedProperty");

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
