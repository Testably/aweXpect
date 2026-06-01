using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithoutAttribute()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<BarAttribute>();

				await That(types).All().Satisfy(t => !Attribute.IsDefined(t!, typeof(BarAttribute), true))
					.And.IsNotEmpty();
				await That(types).DoesNotContain(typeof(BarClass)).And.DoesNotContain(typeof(BarChildClass));
				await That(types.GetDescription())
					.IsEqualTo("types without TypeFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForTypesWithoutAttributeDirectlySet()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Without<BarAttribute>(false);

				await That(types).DoesNotContain(typeof(BarClass)).And.Contains(typeof(BarChildClass));
				await That(types.GetDescription())
					.IsEqualTo("types without direct TypeFilters.Without.AttributeTests.BarAttribute").AsPrefix();
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
		}
	}
}
