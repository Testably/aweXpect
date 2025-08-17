using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPrivateProtected
	{
		public class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PrivateType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivateProtected_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("*is private protected*but it was*").AsWildcard();
			}

			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivateProtected_ShouldSucceed(Type? subject)
			{
				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPrivateProtected_ShouldFail()
			{
				Type? subject = typeof(PrivateProtectedType);

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPrivateProtected_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(PrivateProtectedType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}

			private protected class PrivateProtectedType;
		}
	}
}
