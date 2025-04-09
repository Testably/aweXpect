using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Types
	{
		public sealed class With
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task ShouldFilterForTypesWithAttribute()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().With<BarAttribute>();

					await That(types).IsEqualTo([typeof(BarClass), typeof(BarChildClass),]).InAnyOrder();
					await That(types.GetDescription()).IsEqualTo("types with BarAttribute").AsPrefix();
				}

				[Fact]
				public async Task WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().With<BarAttribute>(false);

					await That(types).HasSingle().Which.IsEqualTo(typeof(BarClass));
					await That(types.GetDescription()).IsEqualTo("types with direct BarAttribute").AsPrefix();
				}

				[Theory]
				[MemberData(nameof(GetFooValues))]
				public async Task WithPredicate_ShouldFilterForTypesWithAttributeMatchingPredicate(int value,
					Type[] expectedTypes)
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().With<FooAttribute>(foo => foo.Value == value);

					await That(types).IsEqualTo(expectedTypes).InAnyOrder();
					await That(types.GetDescription())
						.IsEqualTo("types with FooAttribute matching foo => foo.Value == value").AsPrefix();
				}

				[Fact]
				public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldFilterForTypesWithAttributeDirectlySet()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<FilteredExtensions>()
						.Types().With<FooAttribute>(foo => foo.Value == 2, false);

					await That(types).HasSingle().Which.IsEqualTo(typeof(FooClass2));
					await That(types.GetDescription())
						.IsEqualTo("types with direct FooAttribute matching foo => foo.Value == 2").AsPrefix();
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
		}
	}
}
