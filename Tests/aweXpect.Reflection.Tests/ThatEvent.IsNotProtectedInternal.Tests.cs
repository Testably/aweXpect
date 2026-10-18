using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotProtectedInternal
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventInfoIsProtectedInternal_ShouldFail()
			{
				EventInfo? subject = GetEvent("ProtectedInternalEvent");

				async Task Act()
				{
					await That(subject).IsNotProtectedInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedEvent", "protected")]
			[Arguments("PublicEvent", "public")]
			[Arguments("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName,
				string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenEventInfoIsProtectedInternal_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("ProtectedInternalEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
