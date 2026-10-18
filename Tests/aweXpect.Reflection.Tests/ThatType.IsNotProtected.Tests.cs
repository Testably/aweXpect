namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotProtected_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is protected*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(InternalType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtected_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtected_ShouldFail()
			{
				Type? subject = typeof(ProtectedType);

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtected_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(ProtectedType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
