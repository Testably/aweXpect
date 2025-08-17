using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotProtectedInternal
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
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).AreNotProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not protected internal,
					             but it contained protected internal items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtectedInternal_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is protected internal,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsProtectedInternal_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedInternalEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotProtectedInternal());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
