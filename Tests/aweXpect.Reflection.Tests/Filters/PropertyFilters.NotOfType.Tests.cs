using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class NotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForPropertiesNotOfType()
			{
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().NotOfType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of type PropertyFilters.NotOfType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportChainingToExcludeSeveralTypes()
			{
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().NotOfType<Dummy>().NotOfType<OtherDummy>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties not of type PropertyFilters.NotOfType.Dummy not of type PropertyFilters.NotOfType.OtherDummy in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForPropertiesNotOfType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().NotOfType(unexpectedType);

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of type PropertyFilters.NotOfType.DummyBase in")
					.AsPrefix();
			}
		}

		private class TestClass
		{
			public DummyBase DummyBaseProperty { get; set; } = null!;
			public Dummy DummyProperty { get; set; } = null!;
			public OtherDummy OtherDummyProperty { get; set; } = null!;
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
