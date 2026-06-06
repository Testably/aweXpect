using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllEventsAreNullable_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsContainNonNullableEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all nullable,
					             but it contained non-nullable events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllEventsAreNullable_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all nullable,
					             but it only contained nullable events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventsContainNonNullableEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllEventsAreNullable_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsContainNonNullableEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all nullable,
					             but it contained non-nullable events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
