using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForFieldsNotOfType()
			{
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().WhichAreNotOfType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of type FieldFilters.WhichAreNotOfType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task ShouldSupportChainingToExcludeSeveralTypes()
			{
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().WhichAreNotOfType<Dummy>().WhichAreNotOfType<OtherDummy>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields not of type FieldFilters.WhichAreNotOfType.Dummy not of type FieldFilters.WhichAreNotOfType.OtherDummy in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForFieldsNotOfType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().WhichAreNotOfType(unexpectedType);

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of type FieldFilters.WhichAreNotOfType.DummyBase in")
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
