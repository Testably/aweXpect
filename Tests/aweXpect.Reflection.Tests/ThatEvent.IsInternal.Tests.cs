using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("InternalEvent");

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedEvent", "protected")]
			[InlineData("PublicEvent", "public")]
			[InlineData("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}
	}
}
