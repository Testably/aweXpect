using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllEventsAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsContainObsoleteEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
					             but * of * were

					             Matching items:
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
			public async Task WhenAllEventsAreNotObsolete_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventsContainObsoleteEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!,
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllEventsAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllEventsAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventsContainObsoleteEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!, typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!,
					}
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
					             but * of * were

					             Matching items:
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
