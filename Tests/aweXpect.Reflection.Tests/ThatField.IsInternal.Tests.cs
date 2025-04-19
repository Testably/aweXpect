using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				FieldInfo? subject = GetField("InternalField");

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedField", "protected")]
			[InlineData("PublicField", "public")]
			[InlineData("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

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
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
