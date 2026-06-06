using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainNonVirtualEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(BaseClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyVirtualEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainNonVirtualEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(BaseClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyVirtualEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all virtual,
					             but it only contained virtual events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEventsContainNonVirtualEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(BaseClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyVirtualEvents_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
