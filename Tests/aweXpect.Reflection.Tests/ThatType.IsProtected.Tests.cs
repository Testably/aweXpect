using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(InternalType), "internal")]
			[InlineData(typeof(PublicType), "public")]
			[InlineData(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotProtected_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsProtected_ShouldSucceed()
			{
				Type? subject = typeof(ProtectedType);

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
