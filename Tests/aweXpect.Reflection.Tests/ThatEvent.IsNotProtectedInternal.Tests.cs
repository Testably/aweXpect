using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldFail()
			{
				EventInfo? subject = GetEvent("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldSucceed(string eventName)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldSucceed()
			{
				EventInfo? subject = GetEvent("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedEvent", "protected")]
			[InlineData("PublicEvent", "public")]
			[InlineData("PrivateEvent", "private")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName, string expectedAccessModifier)
			{
				EventInfo? subject = GetEvent(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}
		}
	}
}
