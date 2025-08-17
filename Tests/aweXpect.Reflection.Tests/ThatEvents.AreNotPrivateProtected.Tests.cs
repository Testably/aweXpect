using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotPrivateProtected
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
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldFail()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).AreNotPrivateProtected();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             all are not private protected,
					             but it contained private protected items [
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
			public async Task WhenEventInfoIsNotPrivateProtected_ShouldFail(string eventName)
			{
				Filtered.Events subject = GetEvents(eventName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that *
					             at least one is private protected,
					             but none were
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventInfoIsPrivateProtected_ShouldSucceed()
			{
				Filtered.Events subject = GetEvents("PrivateProtectedEvent");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotPrivateProtected());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
