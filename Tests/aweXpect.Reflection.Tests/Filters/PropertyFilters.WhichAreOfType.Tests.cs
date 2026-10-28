using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreOfType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWhichImplementType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWhichImplementType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

		public sealed class OrOfTypeGenericTests
		{
			[Test]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfExactType<OtherDummy>().OrOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of exact type PropertyFilters.WhichAreOfType.OtherDummy or of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForPropertiesWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfType<OtherDummy>().OrOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of type PropertyFilters.WhichAreOfType.OtherDummy or of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldNotChangeTheFilterItWasCalledOn()
			{
				Reflection.PropertyFilters.PropertiesOfType ofOtherDummy = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfType<OtherDummy>();

				Filtered.Properties ofOtherDummyOrDummyBase = ofOtherDummy.OrOfType<DummyBase>();

				await That(ofOtherDummy).IsEqualTo([
						typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
					])
					.Because("OrOfType must return a new filter instead of changing the one it was called on");
				await That(ofOtherDummy.GetDescription())
					.IsEqualTo("properties of type PropertyFilters.WhichAreOfType.OtherDummy in assembly").AsPrefix();
				await That(ofOtherDummyOrDummyBase).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task FirstCheckIsOfTypeExactly_ShouldAllowDerivedTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfExactType(typeof(OtherDummy)).OrOfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of exact type PropertyFilters.WhichAreOfType.OtherDummy or of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForPropertiesWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreOfType<OtherDummy>().OrOfType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of type PropertyFilters.WhichAreOfType.OtherDummy or of type PropertyFilters.WhichAreOfType.DummyBase in assembly")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

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
