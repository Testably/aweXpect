using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichContainFields
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterByMatchingField()
			{
				Filtered.Types types = In.Types<ClassWithMarkedField, UntaggedClass>()
					.WhichContainFields(fields => fields.With<MarkerAttribute>());

				await That(types).IsEqualTo([typeof(ClassWithMarkedField),]).InAnyOrder();
			}

			[Fact]
			public async Task ShouldUseTheInnerFilterDescription()
			{
				Filtered.Types types = In.AssemblyContaining<MarkerAttribute>().Types()
					.WhichContainFields(fields => fields.With<MarkerAttribute>());

				await That(types.GetDescription())
					.IsEqualTo(
						"types which contain fields with TypeFilters.WhichContainFields.MarkerAttribute at least once ")
					.AsPrefix();
			}
		}

		[AttributeUsage(AttributeTargets.Field)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedField
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			[Marker] public int? MarkedField;
#pragma warning restore CS0649
		}

		private class UntaggedClass
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public int? PlainField;
#pragma warning restore CS0649
		}
	}
}
