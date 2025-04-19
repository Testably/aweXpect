using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldFail()
			{
				EventInfo? subject = GetEvent("InternalEvent");

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}
	}
}
