using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventHandlerInheritsFromType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<Delegate>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type EventHandler,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsOfExactType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<EventHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type EventHandler,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WithType_WhenEventHandlerInheritsFromType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;
				Type unexpectedType = typeof(Delegate);

				async Task Act()
				{
					await That(subject).IsNotOfExactType(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenEventIsOfExactType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;
				Type unexpectedType = typeof(EventHandler);

				async Task Act()
				{
					await That(subject).IsNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type EventHandler,
					             but it was of type EventHandler
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventHandlerInheritsFromType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<Delegate>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type Delegate,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsOfExactType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<EventHandler>());
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
		}
	}
}
