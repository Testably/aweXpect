using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventHandlersOnlyInheritFromType_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<Delegate>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneEventIsNull_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<Delegate>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type Delegate,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneEventIsOfExactType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type EventHandler,
					             but it contained events of exact type EventHandler

					             Not matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOneEventIsOfExactType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				];
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type EventHandler,
					             but it contained events of exact type EventHandler

					             Not matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventHandlersOnlyInheritFromType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<Delegate>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain an event of exact type Delegate,
					             but it only contained events not of exact type Delegate

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneEventIsOfExactType_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<EventHandler>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenEventHandlersOnlyInheritFromType_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<Delegate>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneEventIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type EventHandler,
					             but it contained events of exact type EventHandler

					             Not matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOneEventIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type EventHandler,
					             but it contained events of exact type EventHandler

					             Not matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfExactType.TestClass.EventHandlerEvent
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public static event EventHandler? EventHandlerEvent
			{
				add { }
				remove { }
			}
		}
	}
}
