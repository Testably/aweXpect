using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPublic
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
					=> await That(subject).AreNotPublic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPublic_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
					=> await That(subject).AreNotPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not public,
					             but it contained public items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("InternalEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotPublic_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is public,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPublic_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PublicEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPublic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
