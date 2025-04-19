using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType), "protected")]
			[InlineData(typeof(PublicType), "public")]
			[InlineData(typeof(InternalType), "internal")]
			public async Task WhenTypeIsNotPrivate_ShouldFail(Type? subject, string expectedAccessModifier)
			{
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
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

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
			public async Task WhenTypeIsPrivate_ShouldSucceed()
			{
				Type? subject = typeof(PrivateType);

				async Task Act()
					=> await That(subject).IsPrivate();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
