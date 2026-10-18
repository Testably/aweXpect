namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(PublicType), "public")]
			[Arguments(typeof(InternalType), "internal")]
			public async Task WhenTypeIsNotPrivate_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(InternalType))]
			public async Task WhenTypeIsNotPrivate_ShouldSucceedWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPrivate_ShouldFailWithNegatedAssertion()
			{
				Type? subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not private*but it was*").AsWildcard();
			}

			[Test]
			public async Task WhenTypeIsPrivate_ShouldSucceed()
			{
				Type subject = typeof(PrivateType);

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
