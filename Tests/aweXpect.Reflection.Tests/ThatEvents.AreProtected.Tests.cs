using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreProtected
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
					=> await That(subject).AreProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are protected,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
					=> await That(subject).AreProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("InternalEvent")]
			[InlineData("PublicEvent")]
			[InlineData("PrivateEvent")]
			public async Task WhenEventInfoIsNotProtected_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("ProtectedEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are protected,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
