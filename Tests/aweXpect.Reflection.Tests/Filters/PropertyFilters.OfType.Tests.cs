using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class OfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWhichImplementType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldFilterForPropertiesWhichImplementType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeGenericTests
		{
			[Fact]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfExactType(typeof(OtherDummy)).OrOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of exact type PropertyFilters.OfType.OtherDummy or of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForPropertiesWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfType<OtherDummy>().OrOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of type PropertyFilters.OfType.OtherDummy or of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfExactType(typeof(OtherDummy)).OrOfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of exact type PropertyFilters.OfType.OtherDummy or of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForPropertiesWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfType<OtherDummy>().OrOfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of type PropertyFilters.OfType.OtherDummy or of type PropertyFilters.OfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
			public DummyBase DummyBaseProperty { get; set; }
			public Dummy DummyProperty { get; set; }
			public OtherDummy OtherDummyProperty { get; set; }
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

		private class OtherDummy
		{
		}
	}
}
