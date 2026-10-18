using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventInfoIsInternal_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("InternalEvent");

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedEvent", "protected")]
			[Arguments("PublicEvent", "public")]
			[Arguments("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

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
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

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
			public async Task WhenEventInfoIsInternal_ShouldFail()
			{
				EventInfo? subject = GetEvent("InternalEvent");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Test]
			[Arguments("ProtectedEvent")]
			[Arguments("PublicEvent")]
			[Arguments("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
