using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotPrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				EventInfo? subject = GetEvent("PrivateEvent");

				async Task Act()
					=> await That(subject).IsNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}
		}
	}
}
