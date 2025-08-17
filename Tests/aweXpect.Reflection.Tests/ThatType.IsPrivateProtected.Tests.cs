using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPrivateProtected
	{
		public class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType), "protected")]
			[InlineData(typeof(PublicType), "public")]
			[InlineData(typeof(InternalType), "internal")]
			public async Task WhenTypeIsNotPrivateProtected_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPrivateProtected_ShouldSucceed()
			{
				Type? subject = typeof(PrivateProtectedType);

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).DoesNotThrow();
			}
			
			private protected class PrivateProtectedType;
		}
	}
}
