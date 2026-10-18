namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(InternalType), "internal")]
			[Arguments(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotPublic_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsPublic_ShouldSucceed()
			{
				Type subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(InternalType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotPublic_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsPublic_ShouldFail()
			{
				Type subject = typeof(PublicType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not public*but it was*").AsWildcard();
			}
		}
	}
}
