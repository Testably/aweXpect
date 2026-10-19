namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivate_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is private*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivate_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivate_ShouldFail()
			{
				Type? subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivate_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
