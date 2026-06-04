using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainNonStaticEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainNonStaticEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEventsContainNonStaticEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticEvents_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticEvents_Negated_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetEvents(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
