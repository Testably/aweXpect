using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent", "protected")]
			[InlineData("PublicEvent", "public")]
			[InlineData("InternalEvent", "internal")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).IsPrivateProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
