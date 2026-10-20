using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent", "protected")]
			[Arguments("InternalEvent", "internal")]
			[Arguments("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

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
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

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
			public async Task WhenEventInfoIsPublic_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PublicEvent");

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
			[Arguments("ProtectedEvent")]
			[Arguments("InternalEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPublic_ShouldFail()
			{
				EventInfo? subject = GetEvent("PublicEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
