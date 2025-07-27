using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWithAttribute()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>();

				await That(properties).IsEqualTo([
					typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty))!,
					typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties with PropertyFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForPropertiesWithAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>(false);

				await That(properties).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties with direct PropertyFilters.With.BarAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForPropertiesWithAttributeMatchingPredicate(int value,
				PropertyInfo?[] expectedProperties)
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<FooAttribute>(foo => foo.Value == value);

				await That(properties).IsEqualTo(expectedProperties).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with PropertyFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForPropertiesWithAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<FooAttribute>(foo => foo.Value == 2, false);

				await That(properties).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2)));
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with direct PropertyFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, PropertyInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2)),
							typeof(DummyChild).GetProperty(nameof(DummyChild.MyFooProperty2)),
						]
					},
					{
						3, [typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty3)),]
					},
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWithAttribute()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>().OrWith<FooAttribute>();

				await That(properties).IsEqualTo([
					typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty))!,
					typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2))!,
					typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty3))!,
					typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty))!,
					typeof(DummyChild).GetProperty(nameof(DummyChild.MyFooProperty2))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with PropertyFilters.With.BarAttribute or with PropertyFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForPropertiesWithAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>().OrWith<FooAttribute>(false);

				await That(properties).IsEqualTo([
					typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty))!,
					typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2))!,
					typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty3))!,
					typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with PropertyFilters.With.BarAttribute or with direct PropertyFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForPropertiesWithAttributeMatchingPredicate(int value,
				PropertyInfo?[] expectedProperties)
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>(false).OrWith<FooAttribute>(foo => foo.Value == value);

				await That(properties).IsEqualTo(expectedProperties).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with direct PropertyFilters.With.BarAttribute or with PropertyFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForPropertiesWithAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().With<BarAttribute>(_ => false)
					.OrWith<FooAttribute>(foo => foo.Value == 2, false);

				await That(properties).HasSingle().Which
					.IsEqualTo(typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2)));
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties with PropertyFilters.With.BarAttribute matching _ => false or with direct PropertyFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, PropertyInfo?[]> GetFooValues()
				=> new()
				{
					{
						2, [
							typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)),
							typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty2)),
							typeof(DummyChild).GetProperty(nameof(DummyChild.MyFooProperty2)),
						]
					},
					{
						3, [
							typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)),
							typeof(Dummy).GetProperty(nameof(Dummy.MyFooProperty3)),
						]
					},
				};
		}

		[AttributeUsage(AttributeTargets.Property)]
		private class BarAttribute : Attribute
		{
		}

		private class Dummy
		{
			[Bar]
			public virtual int MyBarProperty { get; set; }

			[Foo(Value = 2)]
			public virtual int MyFooProperty2 { get; set; }

			[Foo(Value = 3)]
			public virtual int MyFooProperty3 { get; set; }
		}

		private class DummyChild : Dummy
		{
			public override int MyBarProperty { get; set; }

			public override int MyFooProperty2 { get; set; }
		}

		[AttributeUsage(AttributeTargets.Property)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}
	}
}
