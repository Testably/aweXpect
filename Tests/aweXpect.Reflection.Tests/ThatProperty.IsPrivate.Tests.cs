using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedProperty", "protected")]
			[InlineData("PublicProperty", "public")]
			[InlineData("InternalProperty", "internal")]
			public async Task WhenPropertyInfoIsNotPrivate_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsPrivate_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("PrivateProperty");

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
