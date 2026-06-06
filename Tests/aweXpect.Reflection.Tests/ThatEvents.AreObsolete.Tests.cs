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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
