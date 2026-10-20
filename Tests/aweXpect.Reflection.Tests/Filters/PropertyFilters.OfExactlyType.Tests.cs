using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class OfExactType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWithType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfExactType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of exact type PropertyFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWithType()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfExactType(typeof(DummyBase));

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties of exact type PropertyFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}
#pragma warning restore CA2263

		public sealed class OrOfTypeGenericTests
		{
			[Test]
			public async Task FirstCheckIsOfType_ShouldNotAllowDerivedTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfType<OtherDummy>().OrOfExactType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of type PropertyFilters.OfExactType.OtherDummy or of exact type PropertyFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldFilterForPropertiesWhichImplementAnyOfTheGivenTypes()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().OfExactType<OtherDummy>().OrOfExactType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties of exact type PropertyFilters.OfExactType.OtherDummy or of exact type PropertyFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class OrOfTypeTests
		{
			[Test]
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
						"properties of exact type PropertyFilters.OfExactType.OtherDummy or of type PropertyFilters.OfExactType.DummyBase in assembly")
					.AsPrefix();
			}

			[Test]
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
						"properties of type PropertyFilters.OfExactType.OtherDummy or of type PropertyFilters.OfExactType.DummyBase in assembly")
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
