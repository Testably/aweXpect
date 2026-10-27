namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNot
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(InternalType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeDoesNotHaveTheAccessModifier_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheAccessModifier_ShouldFail()
			{
				Type subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was public
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingAny_ShouldFail()
			{
				Type subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Any);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have any access modifier,
					             but it was private
					             """);
			}

			[Test]
			public async Task WhenUsingCombinedFlagsAndTypeHasNone_ShouldSucceed()
			{
				Type subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingCombinedFlagsAndTypeHasOne_ShouldFail()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).IsNot(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public or internal,
					             but it was internal
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveTheAccessModifier_ShouldFail()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNot(AccessModifiers.Public));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was internal
					             """);
			}

			[Test]
			public async Task WhenTypeHasTheAccessModifier_ShouldSucceed()
			{
				Type subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNot(AccessModifiers.Public));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
