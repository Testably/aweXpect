using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllEventsAreObsolete_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsContainNonObsoleteEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are obsolete,
					             but only * of * were

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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllEventsAreObsolete_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are obsolete,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventsContainNonObsoleteEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllEventsAreObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are obsolete,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllEventsAreObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsContainNonObsoleteEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!, typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are obsolete,
					             but only * of * were

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
		}
#endif
	}
}
