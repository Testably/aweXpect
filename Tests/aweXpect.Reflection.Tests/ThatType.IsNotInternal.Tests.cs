namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsInternal_ShouldFail()
			{
				Type? subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenTypeIsInternal_ShouldSucceedWithNegatedAssertion()
			{
				Type? subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotInternal_ShouldFailWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is internal*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotInternal_ShouldSucceed(Type? subject)
			{
				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}
	}
}
