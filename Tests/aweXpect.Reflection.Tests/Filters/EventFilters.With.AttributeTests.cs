using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWithAttribute()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>();

				await That(events).IsEqualTo([
					typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo("events with EventFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForEventsWithAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>(false);

				await That(events).HasSingle().Which.IsEqualTo(typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)));
				await That(events.GetDescription())
					.IsEqualTo("events with direct EventFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForEventsWithAttributeMatchingPredicate(int value,
				EventInfo?[] expectedEvents)
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<FooAttribute>(foo => foo.Value == value);

				await That(events).IsEqualTo(expectedEvents).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events with EventFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForEventsWithAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<FooAttribute>(foo => foo.Value == 2, false);

				await That(events).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2)));
				await That(events.GetDescription())
					.IsEqualTo(
						"events with direct EventFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, EventInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2)),
						]
					},
					{
						3, [typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent3)),]
					},
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
			public async Task ShouldFilterForEventsWithAttribute()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>().OrWith<FooAttribute>();

				await That(events).IsEqualTo([
					typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent))!,
					typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2))!,
					typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent3))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events with EventFilters.With.BarAttribute or with EventFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForEventsWithAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>().OrWith<FooAttribute>(false);

				await That(events).IsEqualTo([
					typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent))!,
					typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2))!,
					typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent3))!,
				]).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events with EventFilters.With.BarAttribute or with direct EventFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForEventsWithAttributeMatchingPredicate(int value,
				EventInfo?[] expectedEvents)
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>(false).OrWith<FooAttribute>(foo => foo.Value == value);

				await That(events).IsEqualTo(expectedEvents).InAnyOrder();
				await That(events.GetDescription())
					.IsEqualTo(
						"events with direct EventFilters.With.BarAttribute or with EventFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForEventsWithAttributeDirectlySet()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().With<BarAttribute>(_ => false)
					.OrWith<FooAttribute>(foo => foo.Value == 2, false);

				await That(events).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2)));
				await That(events.GetDescription())
					.IsEqualTo(
						"events with EventFilters.With.BarAttribute matching _ => false or with direct EventFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, EventInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)),
							typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent2)),
						]
					},
					{
						3, [
							typeof(Dummy).GetEvent(nameof(Dummy.MyBarEvent)),
							typeof(Dummy).GetEvent(nameof(Dummy.MyFooEvent3)),
						]
					},
				};
		}

		[AttributeUsage(AttributeTargets.Event)]
		private class BarAttribute : Attribute
		{
		}

		private class Dummy
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
			[Bar] public event EventHandler MyBarEvent;

			[Foo(Value = 2)] public event EventHandler MyFooEvent2;

			[Foo(Value = 3)] public event EventHandler MyFooEvent3;
#pragma warning restore CS8618
		}

		[AttributeUsage(AttributeTargets.Event)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}
	}
}
