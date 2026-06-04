using System;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsOfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfExactType<EventHandler>();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is of exact type EventHandler,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventIsOfExactType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventHandlerInheritsFromExpectedType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<MulticastDelegate>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type MulticastDelegate,
					             but it was of type EventHandler
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsOfExactType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfExactType<EventHandler>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type EventHandler,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenEventHandlerInheritsFromExpectedType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfExactType<MulticastDelegate>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public delegate void CustomHandler();

#pragma warning disable CS0067 // The event is never used
		private class TestClass
		{
			public event EventHandler EventHandlerEvent = null!;
		}
#pragma warning restore CS0067

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEventIsOfExactType_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfExactType(typeof(EventHandler));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventHandlerInheritsFromExpectedType_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfExactType(typeof(MulticastDelegate));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type MulticastDelegate,
					             but it was of type EventHandler
					             """);
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WithMultipleOrOfExactType_ShouldSupportChaining()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<MulticastDelegate>().OrOfType(typeof(Action))
						.OrOfExactType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#pragma warning restore CA2263
	}
}
