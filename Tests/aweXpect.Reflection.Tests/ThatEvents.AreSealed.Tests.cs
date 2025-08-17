using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainNonSealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all sealed,
					             but it contained non-sealed events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(ClassWithSealedMembers.VirtualEvent));

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainNonSealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(ClassWithSealedMembers.VirtualEvent));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all sealed,
					             but it only contained sealed events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
