using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterByMatchingProperty()
			{
				Filtered.Types types = In.Types<ClassWithMarkedProperty, UntaggedClass>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(ClassWithMarkedProperty),]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldSupportQuantifiers()
			{
				Filtered.Types types = In.Types<ClassWithMarkedProperty, UntaggedClass>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>()).Never();

				await That(types).IsEqualTo([typeof(UntaggedClass),]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldUseTheInnerFilterDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain properties with TypeFilters.WhichContainProperties.MarkerAttribute at least once ")
					.AsPrefix();
			}

			[Fact]
			public async Task WithDeclaredOnly_ShouldNotIncludeTypesWithOnlyInheritedMatchingProperty()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedProperty>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>());

				await That(types).IsEmpty();
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeTypesWithInheritedMatchingProperty()
			{
				Filtered.Types types = In.Type<DerivedClassWithInheritedMarkedProperty>()
					.WhichContainProperties(properties => properties.With<MarkerAttribute>(),
						MemberScope.IncludingInherited);

				await That(types).IsEqualTo([typeof(DerivedClassWithInheritedMarkedProperty),]).InAnyOrder();
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
