using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				EventInfo? subject = GetEvent("PrivateEvent");

				async Task Act()
				{
					await That(subject).IsNotPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedEvent", "protected")]
			[Arguments("PublicEvent", "public")]
			[Arguments("InternalEvent", "internal")]
			public async Task WhenEventInfoIsNotPrivate_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenEventInfoIsPrivate_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PrivateEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivate());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
