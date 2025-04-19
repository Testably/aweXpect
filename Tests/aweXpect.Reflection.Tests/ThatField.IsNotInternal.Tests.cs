using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				FieldInfo? subject = GetField("InternalField");

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
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
