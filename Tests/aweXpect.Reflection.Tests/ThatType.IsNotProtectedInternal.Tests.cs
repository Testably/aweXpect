namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotProtectedInternal
	{
		public class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is protected internal*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtectedInternal_ShouldFail()
			{
				Type? subject = typeof(ProtectedInternalType);

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtectedInternal_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(ProtectedInternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}

			protected internal class ProtectedInternalType;
		}
	}
}
