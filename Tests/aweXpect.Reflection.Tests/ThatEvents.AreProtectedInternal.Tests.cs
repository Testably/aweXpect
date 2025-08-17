using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreProtectedInternal
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).AreProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected internal,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).AreProtectedInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected internal,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
