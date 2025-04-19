using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent", "protected")]
			[InlineData("InternalEvent", "internal")]
			[InlineData("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsPublic_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PublicEvent");

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
