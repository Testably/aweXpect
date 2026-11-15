using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForEventsWithoutAttribute()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Without<BarAttribute>();

				await That(events).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetEvent(nameof(Dummy.PlainEvent)))
					.And.DoesNotContain(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)))
					.And.DoesNotContain(typeof(DummyChild).GetEvent(nameof(DummyChild.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo("events without EventFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldFilterForEventsWithoutAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Without<BarAttribute>().Directly();

				await That(events)
					.Contains(typeof(DummyChild).GetEvent(nameof(DummyChild.MyBarEvent)))
					.And.DoesNotContain(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo("events without direct EventFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForEventsWithoutMatchingAttribute()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Without<BarAttribute>(bar => bar.Value == 2);

				await That(events)
					.Contains(typeof(Dummy).GetEvent(nameof(Dummy.PlainEvent)))
					.And.Contains(typeof(Dummy).GetEvent(nameof(Dummy.OtherBarEvent)))
					.And.DoesNotContain(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)))
					.And.DoesNotContain(typeof(DummyChild).GetEvent(nameof(DummyChild.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo("events without EventFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenDirectly_ShouldFilterForEventsWithoutMatchingAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Without<BarAttribute>(bar => bar.Value == 2).Directly();

				await That(events)
					.Contains(typeof(DummyChild).GetEvent(nameof(DummyChild.MyBarEvent)))
					.And.DoesNotContain(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo(
						"events without direct EventFilters.Without.AttributeTests.BarAttribute matching bar => bar.Value == 2")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Events events = In.Type<Dummy>().Events()
					.Which(@event => @event.Name == nameof(Dummy.MyBarEvent))
					.Without<BarAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(events).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that events matching @event => @event.Name == nameof(Dummy.MyBarEvent) without EventFilters.Without.AttributeTests.BarAttribute matching _ => throw new InvalidOperationException("boom") in type EventFilters.Without.AttributeTests.Dummy
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class BarAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0067 // Event is never used
			private class Dummy
			{
				[Bar(Value = 2)] public virtual event Action? MyBarEvent;

				[Bar(Value = 3)] public event Action? OtherBarEvent;

				public event Action? PlainEvent;
			}

			private class DummyChild : Dummy
			{
				public override event Action? MyBarEvent;
			}
#pragma warning restore CS0067
		}
	}
}
