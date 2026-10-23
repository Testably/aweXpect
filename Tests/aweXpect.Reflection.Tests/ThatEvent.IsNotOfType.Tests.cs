using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotOfType
	{
		public delegate void CustomHandler();

		public sealed class Tests
		{
			[Test]
			public async Task WhenEventHandlerInheritsFromType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<Delegate>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type Delegate,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type EventHandler,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsOfDifferentType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsOfType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type EventHandler,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WithType_WhenEventIsOfDifferentType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!;
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).IsNotOfType(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenEventIsOfType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).IsNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type EventHandler,
					             but it was of type EventHandler
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsOfDifferentType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfType<EventHandler>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler,
					             but it was of type ThatEvent.IsNotOfType.CustomHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsOfType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfType<EventHandler>());
				}

				await That(Act).DoesNotThrow();
			}
		}

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
