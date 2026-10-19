namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPrivateProtected
	{
		public class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivateProtected_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is private protected*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivateProtected_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivateProtected_ShouldFail()
			{
				Type? subject = typeof(PrivateProtectedType);

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivateProtected_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(PrivateProtectedType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			private protected class PrivateProtectedType;
		}
	}
}
