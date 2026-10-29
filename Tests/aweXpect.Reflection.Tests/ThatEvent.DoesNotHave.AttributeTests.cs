using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenEventDoesNotHaveAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventHasAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.TestClass.TestEvent
					             """);
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_Directly_ShouldSucceed()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_ShouldFail()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.DerivedClass.InheritedEvent
					             """)
					.Because("the attribute on the overridden base event is inherited, as in the Events().With<TAttribute>() filter");
			}

			[Test]
			public async Task WithType_WhenEventHasAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.TestClass.TestEvent
					             """);
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

			private class BaseClass
			{
				[Foo] public virtual event Action? InheritedEvent;
			}

			private class DerivedClass : BaseClass
			{
				public override event Action? InheritedEvent;
			}
#pragma warning restore CS0067
		}
	}
}
