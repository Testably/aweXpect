using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivate_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPrivate_ShouldFail()
			{
				Type? subject = typeof(PrivateType);

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}
		}
	}
}
