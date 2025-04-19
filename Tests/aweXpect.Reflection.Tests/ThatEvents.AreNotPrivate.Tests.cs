using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPrivate
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
					=> await That(subject).AreNotPrivate();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivate_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
					=> await That(subject).AreNotPrivate();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private,
					             but it contained private items [
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
			[InlineData("InternalEvent")]
			public async Task WhenEventInfoIsNotPrivate_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivate_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivate());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
