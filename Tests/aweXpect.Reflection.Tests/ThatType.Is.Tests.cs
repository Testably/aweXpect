namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class Is
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(InternalType), "internal")]
			[Arguments(typeof(PrivateType), "private")]
			public async Task WhenTypeDoesNotHaveTheAccessModifier_ShouldFail(Type subject,
				string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).Is(AccessModifiers.Public);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			[Arguments(typeof(PublicType), AccessModifiers.Public)]
			[Arguments(typeof(ProtectedType), AccessModifiers.Protected)]
			[Arguments(typeof(InternalType), AccessModifiers.Internal)]
			[Arguments(typeof(PrivateType), AccessModifiers.Private)]
			public async Task WhenTypeHasTheAccessModifier_ShouldSucceed(Type subject, AccessModifiers accessModifier)
			{
				async Task Act()
				{
					await That(subject).Is(accessModifier);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.Public);
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingAny_ShouldSucceed()
			{
				Type subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.Any);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingCombinedFlagsAndTypeHasNone_ShouldFail()
			{
				Type subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is public or internal,
					             but it was private
					             """);
			}

			[Test]
			public async Task WhenUsingCombinedFlagsAndTypeHasOne_ShouldSucceed()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).Is(AccessModifiers.Public | AccessModifiers.Internal);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveTheAccessModifier_ShouldSucceed()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Is(AccessModifiers.Public));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheAccessModifier_ShouldFail()
			{
				Type subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Is(AccessModifiers.Public));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was public
					             """);
			}
		}
	}
}
