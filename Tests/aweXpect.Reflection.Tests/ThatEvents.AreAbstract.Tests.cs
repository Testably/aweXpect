using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainNonAbstractEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all abstract,
					             but it contained non-abstract events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(AbstractClassWithMembers.AbstractEvent));

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainNonAbstractEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(AbstractClassWithMembers.AbstractEvent));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all abstract,
					             but it only contained abstract events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
