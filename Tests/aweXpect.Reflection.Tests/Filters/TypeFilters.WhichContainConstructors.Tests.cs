using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainConstructors
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterByMatchingConstructor()
			{
				Filtered.Types types = In.Types<ClassWithMarkedConstructor, UntaggedClass>()
					.WhichContainConstructors(constructors => constructors.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(ClassWithMarkedConstructor),]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldUseTheInnerFilterDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainConstructors(constructors => constructors.With<MarkerAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain constructors with TypeFilters.WhichContainConstructors.MarkerAttribute at least once ")
					.AsPrefix();
			}
		}

		[AttributeUsage(AttributeTargets.Constructor)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedConstructor
		{
			[Marker]
			public ClassWithMarkedConstructor()
			{
			}
		}

		private class UntaggedClass
		{
		}
	}
}
