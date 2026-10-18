namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsInternal_ShouldSucceed()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(PublicType), "public")]
			[Arguments(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotInternal_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsInternal_ShouldFail()
			{
				Type subject = typeof(InternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not internal*but it was*").AsWildcard();
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotInternal_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
