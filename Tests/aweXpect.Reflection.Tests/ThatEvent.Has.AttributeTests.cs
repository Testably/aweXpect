using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenEventDoesNotHaveAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("NoAttributeEvent")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.MarkerAttribute,
					             but it did not in event Action ThatEvent.Has.AttributeTests.TestClass.NoAttributeEvent
					             """);
			}

			[Test]
			public async Task WhenEventHasAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventHasMatchingAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEventWithValue")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == 42);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventHasNotMatchingAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEventWithValue")!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == 99);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.MarkerAttribute matching attr => attr.Value == 99,
					             but it did not in event Action ThatEvent.Has.AttributeTests.TestClass.TestEventWithValue
					             """);
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_Directly_ShouldFail()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has direct ThatEvent.Has.AttributeTests.MarkerAttribute,
					             but it did not in event Action ThatEvent.Has.AttributeTests.DerivedClass.InheritedEvent
					             """)
					.Because("with Directly only the attributes declared on the overriding event count");
			}

			[Test]
			public async Task WhenEventInheritsAttributeFromOverriddenEvent_ShouldSucceed()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow()
					.Because("the attribute on the overridden base event is inherited, as in the Events().With<TAttribute>() filter");
			}

			[Test]
			public async Task WhenEventInheritsMatchingAttributeFromOverriddenEvent_ShouldSucceed()
			{
				EventInfo subject = typeof(DerivedClass).GetEvent(nameof(DerivedClass.InheritedEvent))!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == 7);
				}

				await That(Act).DoesNotThrow()
					.Because("the predicate is evaluated on the attribute inherited from the overridden base event");
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatEvent.Has.AttributeTests.MarkerAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenEventHasAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				Type attributeType = typeof(MarkerAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class MarkerAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Marker] public event Action? TestEvent;

				[Marker(Value = 42)] public event Action? TestEventWithValue;

				public event Action? NoAttributeEvent;
			}

			private class BaseClass
			{
				[Marker(Value = 7)] public virtual event Action? InheritedEvent;
			}

			private class DerivedClass : BaseClass
			{
				public override event Action? InheritedEvent;
			}
#pragma warning restore CS0067
		}

		public sealed class OrHas
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenEventHasBothAttributes_ShouldSucceed()
				{
					EventInfo subject = typeof(FooBarClass).GetEvent("FooBarEvent")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventHasFirstAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(FooClass).GetEvent("FooEvent")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventHasMatchingAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(FooClass2).GetEvent("FooEvent2")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == 2).OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventHasMatchingSecondAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(BarClass3).GetEvent("BarEvent3")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEventHasNeitherAttribute_ShouldFail()
				{
					EventInfo subject = typeof(BazClass).GetEvent("BazEvent")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has ThatEvent.Has.OrHas.AttributeTests.FooAttribute or ThatEvent.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in event Action ThatEvent.Has.OrHas.AttributeTests.BazClass.BazEvent
						             """);
				}

				[Test]
				public async Task WhenEventHasSecondAttribute_ShouldSucceed()
				{
					EventInfo subject = typeof(BarClass).GetEvent("BarEvent")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenEventHasNotMatchingAttribute_ShouldFail()
				{
					EventInfo subject = typeof(FooClass2).GetEvent("FooEvent2")!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has ThatEvent.Has.OrHas.AttributeTests.FooAttribute matching foo => foo.Value == 5 or ThatEvent.Has.OrHas.AttributeTests.BarAttribute matching bar => bar.Name == "test",
						             but it did not in event Action ThatEvent.Has.OrHas.AttributeTests.FooClass2.FooEvent2
						             """);
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class FooAttribute : Attribute
				{
					public int Value { get; set; }
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class BarAttribute : Attribute
				{
					public string? Name { get; set; }
				}

				[AttributeUsage(AttributeTargets.Event)]
				private class BazAttribute : Attribute
				{
				}

#pragma warning disable CS0067 // Event is never used
				private class FooClass
				{
					[Foo] public event Action? FooEvent;
				}

				private class FooClass2
				{
					[Foo(Value = 2)] public event Action? FooEvent2;
				}

				private class BarClass
				{
					[Bar] public event Action? BarEvent;
				}

				private class BarClass3
				{
					[Bar(Name = "test")] public event Action? BarEvent3;
				}

				private class FooBarClass
				{
					[Foo] [Bar] public event Action? FooBarEvent;
				}

				private class BazClass
				{
					[Baz] public event Action? BazEvent;
				}
#pragma warning restore CS0067
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventDoesNotHaveAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("NoAttributeEvent")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventHasAttribute_ShouldFail()
			{
				EventInfo subject = typeof(TestClass).GetEvent("TestEvent")!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatEvent.Has.NegatedTests.MarkerAttribute,
					             but it did in event Action ThatEvent.Has.NegatedTests.TestClass.TestEvent
					             """);
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

#pragma warning disable CS0067 // Event is never used
			private class TestClass
			{
				[Marker(Value = "EventValue")] public event Action? TestEvent;

				public event Action? NoAttributeEvent;
			}
#pragma warning restore CS0067
		}
	}
}
