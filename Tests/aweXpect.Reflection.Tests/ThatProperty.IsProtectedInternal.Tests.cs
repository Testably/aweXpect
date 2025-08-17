using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyInfoIsProtectedInternal_ShouldSucceed()
			{
				PropertyInfo? subject = GetProperty("ProtectedInternalProperty");

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedProperty", "protected")]
			[InlineData("PublicProperty", "public")]
			[InlineData("PrivateProperty", "private")]
			public async Task WhenPropertyInfoIsNotProtectedInternal_ShouldFail(string propertyName, string expectedAccessModifier)
			{
				PropertyInfo? subject = GetProperty(propertyName);

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected internal,
					             but it was <null>
					             """);
			}
		}
	}
}
