using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WhenAnEventHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching events [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}
#endif

			[Fact]
			public async Task Negated_WhenNoEventHasAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching events [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.NoAttributeEvent
					             ]
					             """);
			}

			[Fact]
			public async Task WhenAnEventHasAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching events [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoEventHasAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)), null,
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class FooAttribute : Attribute
			{
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Foo] public event Action? TestEvent;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}
	}
}
