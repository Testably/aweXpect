using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(InternalType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtected_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

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
			public async Task WhenTypeIsProtected_ShouldFail()
			{
				Type? subject = typeof(ProtectedType);

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
