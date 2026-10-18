using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				EventInfo? subject = GetEvent("ProtectedEvent");

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalEvent", "internal")]
			[Arguments("PublicEvent", "public")]
			[Arguments("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("ProtectedEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
