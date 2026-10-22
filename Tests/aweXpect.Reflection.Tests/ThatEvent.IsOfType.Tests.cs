using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsOfType
	{
		public delegate void CustomHandler();

		public sealed class GenericTests
		{
			[Test]
			public async Task WhenEventHandlerInheritsFromExpectedType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<MulticastDelegate>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfType<EventHandler>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is of type EventHandler,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsOfDifferentType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<CustomHandler>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatEvent.IsOfType.CustomHandler,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsOfExpectedType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Test]
			public async Task WhenEventIsNoneOfTheTypes_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<CustomHandler>().OrOfExactType<Action>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatEvent.IsOfType.CustomHandler or of exact type Action,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsOneOfTheTypes_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<CustomHandler>().OrOfExactType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsOfDifferentType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<CustomHandler>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsOfExpectedType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<EventHandler>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type EventHandler,
					             but it was of type EventHandler
					             """);
			}
		}

#pragma warning disable CS0067 // The event is never used
		private class TestClass
		{
			public event EventHandler EventHandlerEvent = null!;
		}
#pragma warning restore CS0067

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEventIsOfDifferentType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(CustomHandler));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatEvent.IsOfType.CustomHandler,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WhenEventIsOfExpectedType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(EventHandler));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task WhenEventIsNoneOfTheTypes_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<CustomHandler>().OrOfType<Action>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type ThatEvent.IsOfType.CustomHandler or of type Action,
					             but it was of type EventHandler
					             """);
			}

			[Test]
			public async Task WithMultipleOrOfType_ShouldSupportChaining()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfType<CustomHandler>().OrOfType(typeof(Action)).OrOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#pragma warning restore CA2263
	}
}
