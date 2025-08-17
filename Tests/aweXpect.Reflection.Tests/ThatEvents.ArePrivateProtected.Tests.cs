using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class ArePrivateProtected
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).ArePrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are private protected,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).ArePrivateProtected();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are private protected,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
