using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
					=> await That(subject).AreInternal();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).AreInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are internal,
					             but all were
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
