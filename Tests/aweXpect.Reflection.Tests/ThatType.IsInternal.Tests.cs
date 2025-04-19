using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsInternal_ShouldSucceed()
			{
				Type subject = typeof(InternalType);

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(typeof(ProtectedType), "protected")]
			[InlineData(typeof(PublicType), "public")]
			[InlineData(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotInternal_ShouldFail(Type? subject, string expectedAccessModifier)
			{
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
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

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
