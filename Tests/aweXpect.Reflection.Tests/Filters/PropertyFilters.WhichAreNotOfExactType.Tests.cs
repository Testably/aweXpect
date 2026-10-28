using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForPropertiesNotOfExactType()
			{
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().WhichAreNotOfExactType<DummyBase>();

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of exact type PropertyFilters.WhichAreNotOfExactType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForPropertiesNotOfExactType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Properties properties = In.Type<TestClass>()
					.Properties().WhichAreNotOfExactType(unexpectedType);

				await That(properties).IsEqualTo([
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherDummyProperty))!,
				]).InAnyOrder();
				await That(properties.GetDescription())
					.IsEqualTo("properties not of exact type PropertyFilters.WhichAreNotOfExactType.DummyBase in")
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
