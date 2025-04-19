using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsProtected_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}
	}
}
