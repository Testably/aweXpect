using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainSealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not sealed,
					             but it contained sealed events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonSealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainSealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonSealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a sealed event,
					             but it only contained non-sealed events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
