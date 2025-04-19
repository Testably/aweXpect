using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType), "protected")]
			[InlineData(typeof(InternalType), "internal")]
			[InlineData(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotPublic_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPublic_ShouldSucceed()
			{
				Type? subject = typeof(PublicType);

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
