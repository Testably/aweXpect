namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPrivateProtected
	{
		public class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(PublicType), "public")]
			[Arguments(typeof(InternalType), "internal")]
			public async Task WhenTypeIsNotPrivateProtected_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PrivateType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivateProtected_ShouldSucceedWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivateProtected_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PrivateProtectedType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not private protected*but it was*").AsWildcard();
			}

			[Test]
			public async Task WhenTypeIsPrivateProtected_ShouldSucceed()
			{
				Type subject = typeof(PrivateProtectedType);

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			private protected class PrivateProtectedType;
		}
	}
}
