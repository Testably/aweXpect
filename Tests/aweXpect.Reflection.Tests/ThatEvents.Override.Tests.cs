using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class Override
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventsContainNonOverridingEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base event,
					             but it contained events which do not override a base event [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyOverridingEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventsContainNonOverridingEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyOverridingEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             do not all override a base event,
					             but it only contained events which override a base event [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenEventsContainNonOverridingEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(AbstractClassWithMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base event,
					             but it contained events which do not override a base event [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyOverridingEvents_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithSealedMembers)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
