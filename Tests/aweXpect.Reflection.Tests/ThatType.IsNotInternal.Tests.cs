using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsInternal_ShouldFail()
			{
				Type? subject = typeof(InternalType);

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
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(PrivateType))]
			public async Task WhenTypeIsNotInternal_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

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
