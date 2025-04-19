using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				EventInfo? subject = GetEvent("ProtectedEvent");

				async Task Act()
					=> await That(subject).IsNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}
	}
}
