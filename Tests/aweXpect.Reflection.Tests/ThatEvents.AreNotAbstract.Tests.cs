using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainAbstractEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not abstract,
					             but it contained abstract events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonAbstractEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(AbstractClassWithMembers.VirtualEvent));

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainAbstractEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonAbstractEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(e => e.Name == nameof(AbstractClassWithMembers.VirtualEvent));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an abstract event,
					             but it only contained non-abstract events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
