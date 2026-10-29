using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreNotOfType
	{
		public delegate void CustomHandler();

		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoEventIsOfType_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneEventHandlerInheritsFromType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<Delegate>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type Delegate,
					             but 1 of 1 were

					             Matching items:
					             [
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]

					             Collection:
					             [
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneEventIsNull_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneEventIsOfType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent,
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<EventInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenOneEventIsOfType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent,
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoEventIsOfType_ShouldFail()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfType<EventHandler>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is of type EventHandler,
					             but none of 1 were

					             Collection:
					             [
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneEventIsOfType_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfType<EventHandler>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoEventIsOfType_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneEventIsOfType_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent,
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOneEventIsOfType_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type EventHandler,
					             but 1 of 2 were

					             Matching items:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent
					             ]

					             Collection:
					             [
					               event EventHandler ThatEvents.AreNotOfType.TestClass.EventHandlerEvent,
					               event ThatEvents.AreNotOfType.CustomHandler ThatEvents.AreNotOfType.TestClass.CustomHandlerEvent
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

			public static event CustomHandler? CustomHandlerEvent
			{
				add { }
				remove { }
			}
		}
	}
}
