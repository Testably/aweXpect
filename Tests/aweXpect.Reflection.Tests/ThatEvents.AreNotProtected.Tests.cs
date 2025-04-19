using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("InternalEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
					=> await That(subject).AreNotProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected,
					             but it contained protected items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
