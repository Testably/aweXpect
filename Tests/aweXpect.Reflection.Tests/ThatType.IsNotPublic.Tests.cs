using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PrivateType))]
			[InlineData(typeof(InternalType))]
			public async Task WhenTypeIsNotPublic_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("*is public*but it was*").AsWildcard();
			}

			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(InternalType))]
			[InlineData(typeof(PrivateType))]
			public async Task WhenTypeIsNotPublic_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPublic_ShouldFail()
			{
				Type? subject = typeof(PublicType);

				async Task Act()
					=> await That(subject).IsNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPublic_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(PublicType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPublic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
