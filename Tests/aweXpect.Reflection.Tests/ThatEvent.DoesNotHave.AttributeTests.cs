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
			public async Task WithPredicate_WhenEventDoesNotHaveAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenEventHasMatchingAttribute_Directly_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no direct ThatEvent.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.TestClass.TestEvent
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenEventHasMatchingAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.TestClass.TestEvent
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenEventHasNotMatchingAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			public async Task WithPredicate_WhenEventInheritsMatchingAttribute_Directly_ShouldSucceed()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WithPredicate_WhenEventInheritsMatchingAttribute_ShouldFail()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.DerivedClass.InheritedEvent
					             """);
			}

			[Test]
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldFail(int value)
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.MultiEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<MultiAttribute>(multi => multi.Value == value);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.MultiAttribute matching multi => multi.Value == value,
					             but it did in event Action ThatEvent.DoesNotHave.AttributeTests.TestClass.MultiEvent
					             """)
					.Because("every attribute of the event counts, not only the first one");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent(nameof(TestClass.TestEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.DoesNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
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
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Foo(Value = 2)] public event Action? TestEvent;

				public event Action? NoAttributeEvent;

				[Multi(Value = 1)] [Multi(Value = 2)] public event Action? MultiEvent;
			}

			private class BaseClass
			{
				[Foo(Value = 2)] public virtual event Action? InheritedEvent;
			}

			private class DerivedClass : BaseClass
			{
				public override event Action? InheritedEvent;
			}
#pragma warning restore CS0067
		}
	}
}
