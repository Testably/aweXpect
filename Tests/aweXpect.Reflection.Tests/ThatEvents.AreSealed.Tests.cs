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
			[Test]
			public async Task WhenEventsContainNonSealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are sealed,
					             but none of * were

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlySealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(ClassWithSealedMembers.VirtualEvent));

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventsContainNonSealedEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlySealedEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(ClassWithSealedMembers.VirtualEvent));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are sealed,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
