using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotPrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldFail()
			{
				EventInfo? subject = GetEvent("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).IsNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedEvent", "protected")]
			[InlineData("PublicEvent", "public")]
			[InlineData("InternalEvent", "internal")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}
		}
	}
}
