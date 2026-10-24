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
			[Test]
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

			[Test]
			public async Task WhenEventsContainNull_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = [null,];

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are nullable,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenEventsContainNullableEvents_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are nullable,
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
			public async Task WhenAllEventsAreNotNullable_ShouldFail()
			{
				IEnumerable<EventInfo> subject = typeof(ClassWithNonNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is nullable,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
			[Test]
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

			[Test]
			public async Task WhenEventsContainNullableEvents_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = typeof(ClassWithMixedNullableEvents)
					.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are nullable,
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
