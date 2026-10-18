using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedEvent", "protected")]
			[Arguments("PublicEvent", "public")]
			[Arguments("InternalEvent", "internal")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName,
				string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

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
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

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
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PrivateProtectedEvent");

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventInfoIsPrivateProtected_ShouldFail()
			{
				EventInfo? subject = GetEvent("PrivateProtectedEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}
	}
}
