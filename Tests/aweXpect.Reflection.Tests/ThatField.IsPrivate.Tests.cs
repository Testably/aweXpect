using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField", "protected")]
			[InlineData("PublicField", "public")]
			[InlineData("InternalField", "internal")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

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
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PrivateField");

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
