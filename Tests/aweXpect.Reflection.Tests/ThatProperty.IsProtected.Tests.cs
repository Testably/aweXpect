using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalProperty", "internal")]
			[InlineData("PublicProperty", "public")]
			[InlineData("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotProtected_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsProtected_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("ProtectedProperty");

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
