using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class AreOfExactType
	{
		public delegate void CustomHandler();

		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFailWhenEventsInheritFromType()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<MulticastDelegate>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type MulticastDelegate,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllEventsAreOfExactType()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<EventHandler>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WhenAllEventsAreOneOfTheExactTypes_ShouldSucceed()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
					typeof(TestClass).GetEvent(nameof(TestClass.CustomHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<EventHandler>().OrOfExactType<CustomHandler>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventStrictlyInheritsFromOrOfExactType_ShouldFail()
			{
				IEnumerable<EventInfo> subject =
				[
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<Action>().OrOfExactType<MulticastDelegate>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type Action or of exact type MulticastDelegate,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task ShouldFailWhenEventsInheritFromType()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!,
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<MulticastDelegate>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type MulticastDelegate,
					             but it contained not matching events [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task ShouldSucceedWhenAllEventsAreOfExactType()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.EventHandlerEvent))!, typeof(TestClass).GetEvent(nameof(TestClass.OtherEventHandlerEvent))!,
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<EventHandler>();
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
	}
}
