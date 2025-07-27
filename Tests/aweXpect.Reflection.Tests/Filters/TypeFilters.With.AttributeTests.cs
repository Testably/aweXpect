using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class With
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithAttribute()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>();

				await That(types).IsEqualTo([typeof(BarClass), typeof(BarChildClass),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("types with TypeFilters.With.BarAttribute").AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>(false);

				await That(types).HasSingle().Which.IsEqualTo(typeof(BarClass));
				await That(types.GetDescription())
					.IsEqualTo("types with direct TypeFilters.With.BarAttribute").AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForTypesWithAttributeMatchingPredicate(int value,
				Type[] expectedTypes)
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<FooAttribute>(foo => foo.Value == value);

				await That(types).IsEqualTo(expectedTypes).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo(
						"types with TypeFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<FooAttribute>(foo => foo.Value == 2, false);

				await That(types).HasSingle().Which.IsEqualTo(typeof(FooClass2));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with direct TypeFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, Type[]> GetFooValues()
				=> new()
				{
					{
						2, [typeof(FooClass2), typeof(FooChildClass2),]
					},
					{
						3, [typeof(FooClass3),]
					},
				};
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithAttribute()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>().OrWith<FooAttribute>();

				await That(types).IsEqualTo([
					typeof(BarClass), typeof(BarChildClass), typeof(FooClass2), typeof(FooClass3),
					typeof(FooChildClass2),
				]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo(
						"types with TypeFilters.With.BarAttribute or with TypeFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>().OrWith<FooAttribute>(false);

				await That(types).IsEqualTo([
					typeof(BarClass), typeof(BarChildClass), typeof(FooClass2), typeof(FooClass3),
				]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo(
						"types with TypeFilters.With.BarAttribute or with direct TypeFilters.With.FooAttribute")
					.AsPrefix();
			}

			[Theory]
			[MemberData(nameof(GetFooValues))]
			public async Task WithPredicate_ShouldFilterForTypesWithAttributeMatchingPredicate(int value,
				Type[] expectedTypes)
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>(false).OrWith<FooAttribute>(foo => foo.Value == value);

				await That(types).IsEqualTo([typeof(BarClass), ..expectedTypes,]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo(
						"types with direct TypeFilters.With.BarAttribute or with TypeFilters.With.FooAttribute matching foo => foo.Value == value")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().With<BarAttribute>(_ => false).OrWith<FooAttribute>(foo => foo.Value == 2, false);

				await That(types).HasSingle().Which.IsEqualTo(typeof(FooClass2));
				await That(types.GetDescription())
					.IsEqualTo(
						"types with TypeFilters.With.BarAttribute matching _ => false or with direct TypeFilters.With.FooAttribute matching foo => foo.Value == 2")
					.AsPrefix();
			}

			public static TheoryData<int, Type[]> GetFooValues()
				=> new()
				{
					{
						2, [typeof(FooClass2), typeof(FooChildClass2),]
					},
					{
						3, [typeof(FooClass3),]
					},
				};
		}

		[AttributeUsage(AttributeTargets.Class)]
		private class BarAttribute : Attribute
		{
		}

		[Bar]
		private class BarClass
		{
		}

		private class BarChildClass : BarClass
		{
		}

		[AttributeUsage(AttributeTargets.Class)]
		private class FooAttribute : Attribute
		{
			public int Value { get; set; }
		}

		[Foo(Value = 2)]
		private class FooClass2
		{
		}

		[Foo(Value = 3)]
		private class FooClass3
		{
		}

		private class FooChildClass2 : FooClass2
		{
		}
	}
}
