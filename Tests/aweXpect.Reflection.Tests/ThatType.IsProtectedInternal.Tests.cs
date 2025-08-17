using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsProtectedInternal
	{
		public class Tests
		{
			[Theory]
			[InlineData(typeof(ProtectedType), "protected")]
			[InlineData(typeof(PublicType), "public")]
			[InlineData(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotProtectedInternal_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Theory]
			[InlineData(typeof(ProtectedType))]
			[InlineData(typeof(PublicType))]
			[InlineData(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldSucceedWithNegatedAssertion(Type? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected internal,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsProtectedInternal_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(ProtectedInternalType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("*is not protected internal*but it was*").AsWildcard();
			}

			[Fact]
			public async Task WhenTypeIsProtectedInternal_ShouldSucceed()
			{
				Type subject = typeof(ProtectedInternalType);

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			protected internal class ProtectedInternalType;
		}
	}
}
