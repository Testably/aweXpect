using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWithoutAttribute()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>();

				await That(properties).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetProperty(nameof(Dummy.PlainProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)))
					.And.DoesNotContain(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties without PropertyFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForPropertiesWithoutAttributeDirectlySet()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Without<BarAttribute>(false);

				await That(properties)
					.Contains(typeof(DummyChild).GetProperty(nameof(DummyChild.MyBarProperty)))
					.And.DoesNotContain(typeof(Dummy).GetProperty(nameof(Dummy.MyBarProperty)));
				await That(properties.GetDescription())
					.IsEqualTo("properties without direct PropertyFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class BarAttribute : Attribute
			{
			}

			private class Dummy
			{
				[Bar] public virtual int MyBarProperty { get; set; }

				public int PlainProperty { get; set; }
			}

			private class DummyChild : Dummy
			{
				public override int MyBarProperty { get; set; }
			}
		}
	}
}
