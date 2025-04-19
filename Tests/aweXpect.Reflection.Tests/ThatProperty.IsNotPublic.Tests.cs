using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("InternalProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotPublic_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsPublic_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("PublicProperty");

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
