using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainProperties
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterByMatchingProperty()
			{
				Filtered.Types types = In.Types<ClassWithMarkedProperty, UntaggedClass>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(ClassWithMarkedProperty),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeTypesWithInheritedMatchingProperty()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedProperty>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(DerivedClassWithInheritedMarkedProperty),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldSupportQuantifiers()
			{
				Filtered.Types types = In.Types<ClassWithMarkedProperty, UntaggedClass>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>()).Never();

				await That(types).IsEqualTo([typeof(UntaggedClass),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldUseTheInnerFilterDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain properties with TypeFilters.WhichContainProperties.MarkerAttribute at least once ")
					.AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldNotIncludeTypesWithOnlyInheritedMatchingProperty()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedProperty>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>()).Directly();

				await That(types).IsEmpty();
			}
		}

		[AttributeUsage(AttributeTargets.Property)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedProperty
		{
			[Marker] public int Id { get; set; }
		}

		private class UntaggedClass
		{
			public int Id { get; set; }
		}

		private class BaseClassWithMarkedProperty
		{
			[Marker] public int Inherited { get; set; }
		}

		private class DerivedClassWithInheritedMarkedProperty : BaseClassWithMarkedProperty
		{
		}
	}
}
