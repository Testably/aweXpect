using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainStaticEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainStaticEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a static event,
					             but it only contained non-static events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEventsContainStaticEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNonStaticEvents_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
