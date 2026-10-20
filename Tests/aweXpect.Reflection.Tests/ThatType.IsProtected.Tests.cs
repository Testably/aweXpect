namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(InternalType), "internal")]
			[Arguments(typeof(PublicType), "public")]
			[Arguments(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotProtected_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotProtected_ShouldSucceedWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtected_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(ProtectedType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not protected*but it was*").AsWildcard();
			}

			[Test]
			public async Task WhenTypeIsProtected_ShouldSucceed()
			{
				Type subject = typeof(ProtectedType);

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
