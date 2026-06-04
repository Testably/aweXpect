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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
