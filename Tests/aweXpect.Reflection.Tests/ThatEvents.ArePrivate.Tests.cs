using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class ArePrivate
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are private,
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
					=> await That(subject).ArePrivate();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedEvent")]
			[InlineData("PublicEvent")]
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldSucceed(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.ArePrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             not all are private,
					             but all were
					             """).AsWildcard();
			}
		}
	}
}
