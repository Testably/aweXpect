using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not internal,
					             but it contained internal items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).AreNotInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventInfoIsInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("InternalEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

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
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is internal,
					             but none were
					             """).AsWildcard();
			}
		}
	}
}
