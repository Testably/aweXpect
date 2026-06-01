using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
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

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForEventsWithoutAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Without<BarAttribute>(false);

				await That(events)
					.Contains(typeof(DummyChild).GetEvent(nameof(DummyChild.MyBarEvent)))
					.And.DoesNotContain(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo("events without direct EventFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Event)]
			private class BarAttribute : Attribute
			{
			}

#pragma warning disable CS0067 // Event is never used
			private class Dummy
			{
				[Bar] public virtual event Action? MyBarEvent;

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
