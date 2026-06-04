using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreOfType
	{
		public delegate void CustomHandler();

		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenSomeEventsAreNotOfType()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<EventHandler>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type EventHandler,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllEventsAreOfType()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSucceedWithInheritance()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<MulticastDelegate>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllEventsAreOfType_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<EventHandler>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all are of type EventHandler,
					             but it only contained matching events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomeEventsAreNotOfType_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<EventHandler>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task ShouldFailWhenSomeEventsAreNotOfType()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!, typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<EventHandler>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type EventHandler,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllEventsAreOfType()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!, typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSucceedWithInheritance()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<MulticastDelegate>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

#pragma warning disable CS0067 // The event is never used
		private class TestClass
		{
			public event EventHandler EventHandlerEvent = null!;
			public event EventHandler OtherEventHandlerEvent = null!;
			public event CustomHandler CustomHandlerEvent = null!;
		}
#pragma warning restore CS0067

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldSucceedWhenAllEventsAreOfType()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType(typeof(EventHandler));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task WhenAllEventsAreOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<EventHandler>().OrOfType(typeof(CustomHandler));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeEventsAreNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<Action>().OrOfType<EventHandler>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of type Action or of type EventHandler,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#pragma warning restore CA2263
	}
}
