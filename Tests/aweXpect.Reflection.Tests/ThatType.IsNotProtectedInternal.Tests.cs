using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotProtectedInternal
	{
		public class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PrivateType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(InternalType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("*is protected internal*but it was*").AsWildcard();
			}

			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsProtectedInternal_ShouldFail()
			{
				Type? subject = typeof(ProtectedInternalType);

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenTypeIsProtectedInternal_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(ProtectedInternalType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			protected internal class ProtectedInternalType;
		}
	}
}
