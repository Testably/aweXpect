namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsProtectedInternal
	{
		public class Tests
		{
			[Test]
			[Arguments(typeof(ProtectedType), "protected")]
			[Arguments(typeof(PublicType), "public")]
			[Arguments(typeof(PrivateType), "private")]
			public async Task WhenTypeIsNotProtectedInternal_ShouldFail(Type? subject, string expectedAccessModifier)
			{
				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			[Arguments(typeof(ProtectedType))]
			[Arguments(typeof(PublicType))]
			[Arguments(typeof(PrivateType))]
			public async Task WhenTypeIsNotProtectedInternal_ShouldSucceedWithNegatedAssertion(Type? subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsProtectedInternal_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(ProtectedInternalType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not protected internal*but it was*").AsWildcard();
			}

			[Test]
			public async Task WhenTypeIsProtectedInternal_ShouldSucceed()
			{
				Type subject = typeof(ProtectedInternalType);

				async Task Act()
				{
					await That(subject).IsProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			protected internal class ProtectedInternalType;
		}
	}
}
