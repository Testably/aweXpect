using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class ArePublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("InternalEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are public,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPublic_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
					=> await That(subject).ArePublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("InternalEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPublic_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are public,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
