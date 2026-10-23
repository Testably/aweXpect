using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class NotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForFieldsNotOfType()
			{
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().NotOfType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of type FieldFilters.NotOfType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportChainingToExcludeSeveralTypes()
			{
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().NotOfType<Dummy>().NotOfType<OtherDummy>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields not of type FieldFilters.NotOfType.Dummy not of type FieldFilters.NotOfType.OtherDummy in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForFieldsNotOfType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().NotOfType(unexpectedType);

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of type FieldFilters.NotOfType.DummyBase in")
					.AsPrefix();
			}
		}

		private class TestClass
		{
			public DummyBase DummyBaseField = null!;
			public Dummy DummyField = null!;
			public OtherDummy OtherDummyField = null!;
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
