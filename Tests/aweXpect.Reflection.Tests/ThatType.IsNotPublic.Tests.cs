namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPublic_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is public*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(InternalType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotPublic_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPublic_ShouldFail()
			{
				Type? subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsPublic_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
