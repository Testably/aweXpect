using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvents
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenAnEventHasMatchingAttribute_ShouldFail()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenNoEventHasMatchingAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				}.ToTestAsyncEnumerable<EventInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.NoAttributeEvent
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_Directly_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.DerivedClass.InheritedEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.DerivedClass.InheritedEvent
					             ]
					             """)
					.Because("the attribute on the overridden base event is inherited, as in the Events().With<TAttribute>() filter");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<EventInfo?> subject = [typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.NoAttributeEvent,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatEvents.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.NoAttributeEvent,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoEventHasAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenAnEventHasMatchingAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 2 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.NoAttributeEvent,
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenEventInheritsMatchingAttribute_Directly_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WithPredicate_WhenEventInheritsMatchingAttribute_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.DerivedClass.InheritedEvent
					             ]

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.DerivedClass.InheritedEvent
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenNoEventHasMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.NoAttributeEvent)),
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("neither a missing attribute nor one that does not match the predicate counts");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				IEnumerable<EventInfo?> subject = new[]
				{
					typeof(TestClass).GetEvent(nameof(TestClass.TestEvent)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatEvents.DoNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               event Action ThatEvents.DoNotHave.AttributeTests.TestClass.TestEvent
					             ]
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Foo(Value = 2)] public event Action? TestEvent;

				public event Action? NoAttributeEvent;
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
