using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class NotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForFieldsNotOfExactType()
			{
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().NotOfExactType<DummyBase>();

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of exact type FieldFilters.NotOfExactType.DummyBase in")
					.AsPrefix();
			}

			[Test]
			public async Task WithType_ShouldFilterForFieldsNotOfExactType()
			{
				Type unexpectedType = typeof(DummyBase);
				Filtered.Fields fields = In.Type<TestClass>()
					.Fields().NotOfExactType(unexpectedType);

				await That(fields).IsEqualTo([
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherDummyField))!,
				]).InAnyOrder();
				await That(fields.GetDescription())
					.IsEqualTo("fields not of exact type FieldFilters.NotOfExactType.DummyBase in")
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
