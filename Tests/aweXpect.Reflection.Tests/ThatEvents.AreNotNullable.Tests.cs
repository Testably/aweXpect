using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllEventsAreNotNullable_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithNonNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsContainNull_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [null,];

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable events [
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEventsContainNullableEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllEventsAreNotNullable_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithNonNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a nullable event,
					             but it only contained non-nullable events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventsContainNullableEvents_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllEventsAreNotNullable_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithNonNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventsContainNullableEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
