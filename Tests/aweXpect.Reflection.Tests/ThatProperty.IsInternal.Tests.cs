using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsInternal_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("InternalProperty");

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedProperty", "protected")]
			[InlineData("PublicProperty", "public")]
			[InlineData("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotInternal_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}
	}
}
