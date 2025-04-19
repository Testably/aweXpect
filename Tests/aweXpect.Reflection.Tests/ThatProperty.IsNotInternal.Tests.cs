using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldFail()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("ProtectedProperty")]
			[InlineData("PublicProperty")]
			[InlineData("PrivateProperty")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldSucceed(string propertyName)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}
	}
}
