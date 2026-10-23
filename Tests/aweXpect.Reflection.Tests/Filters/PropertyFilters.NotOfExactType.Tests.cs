using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class NotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForPropertiesNotOfExactType()
			{
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().NotOfExactType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of exact type PropertyFilters.NotOfExactType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForPropertiesNotOfExactType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().NotOfExactType(unexpectedType);

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of exact type PropertyFilters.NotOfExactType.DummyBase in")
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
