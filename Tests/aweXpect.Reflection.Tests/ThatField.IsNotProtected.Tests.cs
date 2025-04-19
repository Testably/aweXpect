using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				FieldInfo? subject = GetField("ProtectedField");

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
