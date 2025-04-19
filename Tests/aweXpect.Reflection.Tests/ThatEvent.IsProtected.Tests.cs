using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalEvent", "internal")]
			[InlineData("PublicEvent", "public")]
			[InlineData("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("ProtectedEvent");

				async Task Act()
					=> await That(subject).IsProtected();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
